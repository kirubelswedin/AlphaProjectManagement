export function initWysiwygEditor(editorId, toolbarId, textareaId, content) {
	const textarea = document.querySelector(textareaId);
	const editorElement = document.querySelector(editorId);
	const toolbarElement = document.querySelector(toolbarId);

	if (!editorElement || !textarea) {
		console.error("Required elements not found:", { editorId, textareaId });
		return;
	}

	editorElement.innerHTML = "";

	const quill = new Quill(editorElement, {
		modules: {
			syntax: true,
			toolbar: toolbarElement,
		},
		placeholder: "Type something...",
		theme: "snow",
	});

	quill.setText("");

	if (content && content.trim()) {
		quill.root.innerHTML = content;
	}

	quill.on("text-change", () => {
		textarea.value = quill.root.innerHTML;
	});
}
