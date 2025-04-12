function initWysiwygEditor(
	editorSelector,
	toolbarSelector,
	textareaSelector,
	initialContent = ""
) {
	const editor = document.querySelector(editorSelector);
	const toolbar = document.querySelector(toolbarSelector);
	const textarea = document.querySelector(textareaSelector);

	if (!editor || !toolbar || !textarea) return;

	const quill = new Quill(editor, {
		theme: "snow",
		modules: {
			toolbar: toolbar,
		},
	});

	// Set initial content if provided
	if (initialContent) {
		quill.root.innerHTML = initialContent;
	}

	// Update textarea when content changes
	quill.on("text-change", () => {
		textarea.value = quill.root.innerHTML;
	});
}

// Make function globally available
window.initWysiwygEditor = initWysiwygEditor;
