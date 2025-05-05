
const notificationManager = {
	init() {
		this.initSignalR();
		this.updateRelativeTimes();
		setInterval(() => this.updateRelativeTimes(), 60000);

		if ("Notification" in window) {
			Notification.requestPermission();
		}
	},

	initSignalR() {
		this.connection = new signalR.HubConnectionBuilder()
			.withUrl("/notificationHub")
			.withAutomaticReconnect()
			.build();

		// Listen for new notifications and dismissed notifications
		this.connection.on("ReceiveNotification", n => this.handleNewNotification(n));
		this.connection.on("NotificationDismissed", id => this.removeNotification(id));

		this.connection.start().catch(() => 
			showToast("error", "Failed to connect to notifications")
		);
	},

	handleNewNotification(notification) {
		const container = document.querySelector(".dropdown-content");
		const header = container?.querySelector(".dropdown-header");
		const emptyMessage = container?.querySelector(".empty-message");

		if (emptyMessage) emptyMessage.remove();

		// Create and add the notification to the dropdown
		const notificationElement = this.createNotificationElement(notification);
		if (container && header) header.insertAdjacentElement("afterend", notificationElement);

		this.updateNotificationCount();

		if (Notification.permission === "granted") {
			new Notification("New Notification", {
				body: notification.message,
				icon: notification.imageUrl,
			});
		}
	},

	// Create notification element
	createNotificationElement({ id, imageUrl, message }) {
		const el = document.createElement("div");
		el.className = "dropdown-item";
		el.dataset.id = id;
		el.dataset.read = "false";
		el.innerHTML = `
			<div class="content-item">
				<img src="${imageUrl}" alt="Notification image" class="notification-image" />
				<div class="notification-content">
					<div class="notification-message">${message}</div>
					<div class="notification-time time" data-created="${new Date().toISOString()}">
						Just now
					</div>
				</div>
				<button class="notification-close" type="button">
					<i class="fa-solid fa-xmark"></i>
				</button>
			</div>
		`;
		el.querySelector(".notification-close").onclick = e => this.dismissNotification(id, e);
		return el;
	},

	// Update the "x min ago" text for all notifications
	updateRelativeTimes() {
		const now = new Date();
		document.querySelectorAll(".time").
		forEach(el => {
			const timestamp = el.getAttribute("data-created");
			if (!timestamp) return;
			
			try {
				const created = new Date(timestamp);
				const diff = now - created;
				const 
					diffSeconds = Math.floor(diff / 1000), 
					diffMinutes = Math.floor(diffSeconds / 60), 
					diffHours = Math.floor(diffMinutes / 60),
					diffDays = Math.floor(diffHours / 24), 
					diffWeeks = Math.floor(diffDays / 7);
				let relativeTime = "";
				
				if (diffSeconds < 30) 
					relativeTime = "Just now";
				else if (diffMinutes < 1) 
					relativeTime = "Less than a minute ago";
				else if (diffMinutes < 60) 
					relativeTime = `${diffMinutes} min ago`;
				else if (diffHours < 2) 
					relativeTime = "1 hour ago";
				else if (diffHours < 24) 
					relativeTime = `${diffHours} hours ago`;
				else if (diffDays < 2) 
					relativeTime = "1 day ago";
				else if (diffDays < 7) 
					relativeTime = `${diffDays} days ago`;
				else 
					relativeTime = `${diffWeeks} weeks ago`;
				
				el.textContent = relativeTime;
			} catch {
				showToast("error", "Failed to update notification time");
			}
		});
	},

	// Remove a notification when "x" is clicked
	async dismissNotification(id, event) {
		event?.stopPropagation();
		try {
			const response = await fetch(`/api/notifications/dismiss/${id}`, { method: "POST" });
			if (response.ok) this.removeNotification(id);
			else showToast("error", "Failed to dismiss notification");
		} catch {
			showToast("error", "Failed to dismiss notification");
		}
	},
	
	removeNotification(id) {
		document.querySelectorAll(`.dropdown-item[data-id="${id}"]`).
		forEach(item => 
			item.remove());
		// Update counter after removal
		requestAnimationFrame(() => 
			this.updateNotificationCount());
	},

	// Update the notification counters and empty message
	updateNotificationCount() {
		const container = document.querySelector(".dropdown-content");
		if (!container) return;
		
		const notificationItems = Array.from(container.children).filter(
			item => 
				item.classList.contains("dropdown-item") && 
				!item.classList.contains("empty-message")
		);
		const totalCount = notificationItems.length;

		// Update counters in header and dropdown
		const setCounter = (selector, value, display) => {
			const headerCounter = document.querySelector(selector);
			if (headerCounter) {
				headerCounter.textContent = value;
				headerCounter.style.display = totalCount > 0 ? display : "none";
			}
		};
		setCounter(".notification-counter", totalCount, "flex");
		setCounter(".notification-number", totalCount, "inline-block");

		// Show "No notifications" if empty
		if (totalCount === 0) {
			container.innerHTML = "";
			const emptyMessage = document.createElement("div");
			emptyMessage.className = "dropdown-item empty-message";
			emptyMessage.textContent = "No notifications";
			container.appendChild(emptyMessage);
		}
	},
};

document.addEventListener("DOMContentLoaded", () => notificationManager.init());