/* prettier-ignore */

/**
 * Notification Manager
 * Manages real-time notifications using SignalR
 */
const notificationManager = {
	connection: null,

	init() {
		this.initSignalR();
		this.updateRelativeTimes();
		setInterval(() => this.updateRelativeTimes(), 60000); // Update times every minute
	},

	initSignalR() {
		this.connection = new signalR.HubConnectionBuilder()
			.withUrl("/notificationHub")
			.withAutomaticReconnect()
			.build();

		this.connection.on("ReceiveNotification", (notification) => {
			this.handleNewNotification(notification);
		});

		this.connection
			.start()
			.then(() => console.log("SignalR Connected"))
			.catch((err) => console.error("SignalR Connection Error: ", err));
	},

	handleNewNotification(notification) {
		// Find the notifications dropdown content
		const notificationsContainer = document.querySelector(
			"#notification-dropdown .dropdown-content"
		);
		if (!notificationsContainer) {
			console.error("Notification container not found");
			return;
		}

		// Update notification count
		const notificationNumber = document.querySelector(".notification-number");
		if (notificationNumber) {
			const currentCount = parseInt(notificationNumber.textContent) || 0;
			notificationNumber.textContent = currentCount + 1;
		}

		// Show indicator
		const indicator = document.querySelector(".notification-indicator");
		if (indicator) {
			indicator.style.display = "block";
		}

		// If there's an empty message, remove it
		const emptyMessage = notificationsContainer.querySelector(".empty-message");
		if (emptyMessage) {
			const emptyItem = emptyMessage.closest(".dropdown-item");
			if (emptyItem) {
				emptyItem.remove();
			}
		}

		// Create a new notification item
		const newItem = document.createElement("div");
		newItem.className = "dropdown-item";
		newItem.innerHTML =
		`
            <div class="notification-item" data-id="${notification.id}" data-read="false">
                <img src="${notification.image}" alt="Notification image" class="notification-image" />
                <div class="notification-content">
                    <div class="notification-message">${notification.message}</div>
                    <div class="notification-time time" data-created="${new Date().toISOString()}">
                        Just now
                    </div>
                </div>
                <button class="notification-close" onclick="notificationManager.dismissNotification('${notification.id}', event)">
                    <i class="fa-solid fa-xmark"></i>
                </button>
            </div>
        `;

		// Add to the beginning of the container (newest first)
		if (notificationsContainer.firstChild) {
			notificationsContainer.insertBefore(
				newItem,
				notificationsContainer.firstChild
			);
		} else {
			notificationsContainer.appendChild(newItem);
		}

		// Show desktop notification if permission is granted
		if ("Notification" in window && Notification.permission === "granted") {
			new Notification("New Notification", {
				body: notification.message,
				icon: notification.image,
			});
		}
	},

	updateRelativeTimes() {
		const elements = document.querySelectorAll(".time");
		const now = new Date();

		elements.forEach((el) => {
			const timestamp = el.getAttribute("data-created");
			if (!timestamp) return;

			try {
				const created = new Date(timestamp);
				// Convert UTC to local time
				const localCreated = new Date(created.getTime() - created.getTimezoneOffset() * 60000);
				const diff = now - localCreated;
				const diffSeconds = Math.floor(diff / 1000);
				const diffMinutes = Math.floor(diffSeconds / 60);
				const diffHours = Math.floor(diffMinutes / 60);
				const diffDays = Math.floor(diffHours / 24);
				const diffWeeks = Math.floor(diffDays / 7);
				let relativeTime = "";

				if (diffSeconds < 30) {
					relativeTime = "Just now";
				} else if (diffMinutes < 1) {
					relativeTime = "Less than a minute ago";
				} else if (diffMinutes < 60) {
					relativeTime = diffMinutes + " min ago";
				} else if (diffHours < 2) {
					relativeTime = "1 hour ago";
				} else if (diffHours < 24) {
					relativeTime = diffHours + " hours ago";
				} else if (diffDays < 2) {
					relativeTime = "1 day ago";
				} else if (diffDays < 7) {
					relativeTime = diffDays + " days ago";
				} else {
					relativeTime = diffWeeks + " weeks ago";
				}

				el.textContent = relativeTime;
			} catch (error) {
				console.error("Error parsing date:", timestamp, error);
			}
		});
	},

	async dismissNotification(id, event) {
		if (event) {
			event.stopPropagation();
		}

		try {
			const response = await fetch(`/api/notifications/dismiss/${id}`, {
				method: "POST",
				headers: {
					"Content-Type": "application/json",
				},
			});

			if (response.ok) {
				this.removeNotification(id);
			} else {
				console.error("Failed to dismiss notification");
			}
		} catch (error) {
			console.error("Error dismissing notification:", error);
		}
	},

	removeNotification(id) {
		const notification = document.querySelector(
			`.notification-item[data-id="${id}"]`
		);
		if (!notification) return;

		// Find parent dropdown-item
		const dropdownItem = notification.closest(".dropdown-item");
		if (dropdownItem) {
			dropdownItem.remove();
		} else {
			notification.remove();
		}

		this.updateNotificationCount();
	},

	updateNotificationCount() {
		// Update count in header
		const notificationItems = document.querySelectorAll(
			".notification-item[data-id]"
		);
		const count = notificationItems.length;

		const notificationNumber = document.querySelector(".notification-number");
		if (notificationNumber) {
			notificationNumber.textContent = count;

			if (count === 0) {
				// If no notifications, add empty message
				const notificationsContainer = document.querySelector(
					"#notification-dropdown .dropdown-content"
				);
				if (notificationsContainer) {
					notificationsContainer.innerHTML =
						'<div class="dropdown-item empty-message"><span>No notifications</span></div>';
				}
			}
		}

		// Update indicator
		const indicator = document.querySelector(".notification-indicator");
		if (indicator) {
			indicator.style.display = count > 0 ? "block" : "none";
		}
	},
};

// Initialize notification manager when page loads
document.addEventListener("DOMContentLoaded", () => {
	window.notificationManager = notificationManager;
	window.notificationManager.init();

	// Request notification permission
	if ("Notification" in window && Notification.permission === "default") {
		Notification.requestPermission();
	}
});

export default {
	init: () => {
		if (!window.notificationManager) {
			window.notificationManager = notificationManager;
			window.notificationManager.init();
		}
	},
};
