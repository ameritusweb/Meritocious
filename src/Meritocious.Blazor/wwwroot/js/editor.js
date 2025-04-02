// Functions for the Markdown Editor

// Insert text at cursor position in a textarea
window.insertTextAtCursor = function (prefix, suffix, placeholder, newLine) {
    const activeElement = document.activeElement;
    
    if (activeElement.tagName === 'TEXTAREA') {
        const startPos = activeElement.selectionStart;
        const endPos = activeElement.selectionEnd;
        const text = activeElement.value;
        const selectedText = text.substring(startPos, endPos);
        
        let textToInsert = '';
        
        if (selectedText.length > 0) {
            // Use the selected text instead of the placeholder
            textToInsert = prefix + selectedText + suffix;
        } else {
            // Use the placeholder
            textToInsert = prefix + placeholder + suffix;
        }
        
        // Add a newline at the beginning if requested and not at the start of the text
        if (newLine && startPos > 0 && text.charAt(startPos - 1) !== '\n') {
            textToInsert = '\n' + textToInsert;
        }
        
        // Update the textarea value
        activeElement.value = 
            text.substring(0, startPos) + 
            textToInsert + 
            text.substring(endPos);
        
        // Trigger change event manually (for Blazor binding)
        const event = new Event('change', { bubbles: true });
        activeElement.dispatchEvent(event);
        
        // Set selection to the end of the inserted text
        const newCursorPos = startPos + textToInsert.length;
        activeElement.selectionStart = newCursorPos;
        activeElement.selectionEnd = newCursorPos;
        
        // Focus back on the textarea
        activeElement.focus();
    }
};

// Insert text directly at the selection
window.insertTextAtSelection = function (text) {
    const activeElement = document.activeElement;
    
    if (activeElement.tagName === 'TEXTAREA') {
        const startPos = activeElement.selectionStart;
        const endPos = activeElement.selectionEnd;
        const currentText = activeElement.value;
        
        // Replace the selected text with the provided text
        activeElement.value = 
            currentText.substring(0, startPos) + 
            text + 
            currentText.substring(endPos);
        
        // Trigger change event manually (for Blazor binding)
        const event = new Event('change', { bubbles: true });
        activeElement.dispatchEvent(event);
        
        // Set cursor position after the inserted text
        const newCursorPos = startPos + text.length;
        activeElement.selectionStart = newCursorPos;
        activeElement.selectionEnd = newCursorPos;
        
        // Focus back on the textarea
        activeElement.focus();
    }
};

// Focus on an element by ID
window.focusElement = function (elementId) {
    const element = document.getElementById(elementId);
    if (element) {
        element.focus();
    }
};