/**
 * Handles notifications functionality
 */

class NotificationManager {
    constructor() {
        this.connection = null;
        this.notificationsList = document.querySelector('.dropdown-content');
        this.notificationCount = document.querySelector('.notification-number');
    }

    init() {
        this.initSignalR();
        this.loadNotifications();
        this.setupEventListeners();
        this.updateRelativeTimes();
        setInterval(() => this.updateRelativeTimes(), 60000);
    }

    async initSignalR() {
        try {
            this.connection = new signalR.HubConnectionBuilder()
                .withUrl("/notificationHub")
                .withAutomaticReconnect()
                .build();

            this.connection.on("ReceiveNotification", (notification) => {
                this.handleNewNotification(notification);
            });

            this.connection.on("NotificationRead", (notificationId) => {
                this.handleNotificationRead(notificationId);
            });

            this.connection.on("NotificationDismissed", (notificationId) => {
                this.handleNotificationDismissed(notificationId);
            });

            await this.connection.start();
            console.log("SignalR Connected");
        } catch (err) {
            console.error("SignalR Connection Error: ", err);
        }
    }

    async handleNewNotification(notification) {
        // Reload notifications to get the latest
        await this.loadNotifications();
        
        // Show notification if supported
        if ("Notification" in window && Notification.permission === "granted") {
            new Notification(notification.title, {
                body: notification.message,
                icon: notification.image
            });
        }
    }

    async loadNotifications() {
        try {
            const response = await fetch('/api/notifications');
            if (!response.ok) throw new Error('Failed to load notifications');
            
            const data = await response.json();
            if (data.success && data.notifications) {
                await this.updateNotificationsList(data.notifications);
                await this.updateUnreadCount();
            }
        } catch (error) {
            console.error('Error loading notifications:', error);
        }
    }

    updateRelativeTimes() {
        const timeElements = document.querySelectorAll('.notification-time');
        timeElements.forEach(element => {
            const timestamp = new Date(element.getAttribute('data-timestamp'));
            element.textContent = this.getTimeAgo(timestamp);
        });
    }

    getTimeAgo(date) {
        const seconds = Math.floor((new Date() - date) / 1000);
        
        let interval = Math.floor(seconds / 31536000);
        if (interval > 1) return interval + ' years ago';
        
        interval = Math.floor(seconds / 2592000);
        if (interval > 1) return interval + ' months ago';
        
        interval = Math.floor(seconds / 86400);
        if (interval > 1) return interval + ' days ago';
        
        interval = Math.floor(seconds / 3600);
        if (interval > 1) return interval + ' hours ago';
        
        interval = Math.floor(seconds / 60);
        if (interval > 1) return interval + ' minutes ago';
        
        if(seconds < 10) return 'just now';
        
        return Math.floor(seconds) + ' seconds ago';
    }

    async updateNotificationsList(notifications) {
        if (!this.notificationsList) return;
        
        if (!notifications || notifications.length === 0) {
            this.notificationsList.innerHTML = '<div class="dropdown-item empty-message"><span>No notifications</span></div>';
            return;
        }

        const notificationsHtml = notifications.map(notification => `
            <div class="notifications">
                <div class="notification-item" data-id="${notification.id}" data-read="${notification.isRead.toString().toLowerCase()}">
                    <img src="${notification.image}" alt="Notification image" class="notification-image" />
                    <div class="notification-content" onclick="notificationManager.markAsRead('${notification.id}', event)">
                        <span class="notification-message">${notification.message}</span>
                        <span class="notification-time" data-createdAt="${notification.createdAt}">${this.getTimeAgo(new Date(notification.createdAt))}</span>
                    </div>
                    <button class="notification-close" onclick="notificationManager.dismissNotification('${notification.id}', event)">
                        <i class="fa-solid fa-xmark"></i>
                    </button>
                </div>
            </div>
        `).join('');

        this.notificationsList.innerHTML = notificationsHtml;
    }

    async updateUnreadCount() {
        try {
            const response = await fetch('/api/notifications/unread-count');
            if (!response.ok) throw new Error('Failed to get unread count');
            
            const data = await response.json();
            if (this.notificationCount) {
                this.notificationCount.textContent = data.count;
                
                // Uppdatera synligheten av räknaren
                if (data.count > 0) {
                    this.notificationCount.style.display = 'flex';
                } else {
                    this.notificationCount.style.display = 'none';
                }
            }
        } catch (error) {
            console.error('Error updating unread count:', error);
        }
    }

    async markAsRead(id, event) {
        if (event) {
            event.stopPropagation();
        }
        
        try {
            const response = await fetch(`/api/notifications/read/${id}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            if (!response.ok) throw new Error('Failed to mark notification as read');

            // Uppdatera UI direkt
            this.handleNotificationRead(id);
        } catch (error) {
            console.error('Error marking notification as read:', error);
        }
    }

    handleNotificationRead(notificationId) {
        const notification = document.querySelector(`.notification-item[data-id="${notificationId}"]`);
        if (notification) {
            notification.classList.add('read');
            this.updateUnreadCount();
        }
    }

    async dismissNotification(id, event) {
        event.stopPropagation(); 
        
        try {
            const response = await fetch(`/api/notifications/dismiss/${id}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            if (!response.ok) throw new Error('Failed to dismiss notification');

            // UI uppdateras via SignalR callback
        } catch (error) {
            console.error('Error dismissing notification:', error);
        }
    }

    handleNotificationDismissed(notificationId) {
        const notification = document.querySelector(`.notification-item[data-id="${notificationId}"]`);
        if (notification) {
            notification.remove();
            this.updateUnreadCount();
            
            // Om det var den sista notifikationen, visa "No notifications"
            if (!this.notificationsList.querySelector('.notification-item')) {
                this.notificationsList.innerHTML = '<div class="dropdown-item empty-message"><span>No notifications</span></div>';
            }
        }
    }

    setupEventListeners() {
        // Request notification permission
        if ("Notification" in window && Notification.permission === "default") {
            Notification.requestPermission();
        }
    }
}

export default new NotificationManager();

window.notificationManager = new NotificationManager();