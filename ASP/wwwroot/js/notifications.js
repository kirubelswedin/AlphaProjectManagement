const notificationManager = {
	init() {
		this.initSignalR();
		this.updateRelativeTimes();
		setInterval(() => this.updateRelativeTimes(), 60000);

		if ("Notification" in window) {
			Notification.requestPermission();
		}
	},

	// Set up SignalR connection and event handlers
	initSignalR() {
		this.connection = new signalR.HubConnectionBuilder()
			.withUrl("/notificationHub")
			.withAutomaticReconnect()
			.build();

		this.connection.on("ReceiveNotification", (notification) =>
			this.handleNewNotification(notification)
		);

		this.connection.on("NotificationDismissed", (notificationId) => {
			this.removeNotification(notificationId);
		});

		this.connection
			.start()
			.catch((err) => showToast("error", "Failed to connect to notifications"));
	},

	handleNewNotification(notification) {
		// Create notification element
		const notificationElement = this.createNotificationElement({
			id: notification.id,
			imageUrl: notification.imageUrl,
			message: notification.message,
		});

		// Get the notifications container
		const container = document.querySelector(".dropdown-content");
		const header = container?.querySelector(".dropdown-header");
		const emptyMessage = container?.querySelector(".empty-message");

		// Remove empty message if it exists
		if (emptyMessage) {
			emptyMessage.remove();
		}

		// Add the new notification after the header
		if (container && header) {
			header.insertAdjacentElement("afterend", notificationElement);
		}

		// Update both notification counters
		this.updateNotificationCount();

		// Show browser notification
		if (Notification.permission === "granted") {
			new Notification("New Notification", {
				body: notification.message,
				icon: notification.imageUrl,
			});
		}
	},

	// Create notification element from notification data
	createNotificationElement(notification) {
		const newItem = document.createElement("div");
		newItem.className = "dropdown-item";
		newItem.setAttribute("data-id", notification.id);
		newItem.setAttribute("data-read", "false");
		newItem.innerHTML = `
			<div class="content-item">
				<img src="${
					notification.imageUrl
				}" alt="Notification image" class="notification-image" />
				<div class="notification-content">
					<div class="notification-message">${notification.message}</div>
					<div class="notification-time time" data-created="${new Date().toISOString()}">
						Just now
					</div>
				</div>
				<button class="notification-close" onclick="notificationManager.dismissNotification('${
					notification.id
				}', event)">
					<i class="fa-solid fa-xmark"></i>
				</button>
			</div>
		`;

		return newItem;
	},

	updateRelativeTimes() {
		const elements = document.querySelectorAll(".time");
		const now = new Date();

		elements.forEach((el) => {
			const timestamp = el.getAttribute("data-created");
			if (!timestamp) return;

			try {
				const created = new Date(timestamp);
				const localCreated = new Date(
					created.getTime() - created.getTimezoneOffset() * 60000
				);
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
				showToast("error", "Failed to update notification time");
			}
		});
	},

	// Dismiss a notification
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
				showToast("error", "Failed to dismiss notification");
			}
		} catch (error) {
			showToast("error", "Failed to dismiss notification");
		}
	},

	removeNotification(id) {
		const container = document.querySelector(".dropdown-content");
		if (!container) {
			return;
		}

		// Find notification items with the given ID
		const notificationItems = document.querySelectorAll(
			`.dropdown-item[data-id="${id}"]`
		);

		notificationItems.forEach((item) => {
			item.remove();
		});

		requestAnimationFrame(() => {
			this.updateNotificationCount();
		});
	},

	updateNotificationCount() {
		const container = document.querySelector(".dropdown-content");
		if (!container) {
			return;
		}

		// Get all actual notification items
		const notificationItems = Array.from(container.children).filter(
			(item) =>
				item.classList.contains("dropdown-item") &&
				!item.classList.contains("empty-message")
		);

		const totalCount = notificationItems.length;

		// Update header counter
		const headerCounter = document.querySelector(".notification-counter");
		if (headerCounter) {
			headerCounter.textContent = totalCount.toString();
			headerCounter.style.display = totalCount > 0 ? "flex" : "none";
		}

		// Update dropdown counter
		const dropdownCounter = document.querySelector(".notification-number");
		if (dropdownCounter) {
			dropdownCounter.textContent = totalCount.toString();
			dropdownCounter.style.display = totalCount > 0 ? "inline-block" : "none";
		}

		if (totalCount === 0) {
			container.innerHTML = "";
			// Add the empty message
			const emptyMessage = document.createElement("div");
			emptyMessage.className = "dropdown-item empty-message";
			emptyMessage.textContent = "No notifications";
			container.appendChild(emptyMessage);
		}
	},
};

document.addEventListener("DOMContentLoaded", () => {
	notificationManager.init();
});
