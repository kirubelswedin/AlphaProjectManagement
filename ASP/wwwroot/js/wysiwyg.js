export function initWysiwygEditor(editorId, toolbarId, textareaId, content) {
	const textarea = document.querySelector(textareaId);
	const editorElement = document.querySelector(editorId);

	if (editorElement) {
		editorElement.innerHTML = "";
	}

	const quill = new Quill(editorId, {
		modules: {
			syntax: true,
			toolbar: toolbarId,
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
