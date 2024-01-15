function getAntiForgeryToken() {
    const elements = document.getElementsByName('__RequestVerificationToken');
    if (elements.length > 0) {
        return elements[0].value
    }

    console.warn('XSRF token not found!');
    return null;
}
