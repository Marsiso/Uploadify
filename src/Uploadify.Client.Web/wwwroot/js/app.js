window.culture = {
    get: () => window.localStorage['culture'],
    set: (value) => window.localStorage['culture'] = value
};

window.triggerFileDownload = (filename, url) => {
    const anchorElement = document.createElement('a');
    anchorElement.href = url;
    anchorElement.download = filename ?? '';
    anchorElement.click();
    anchorElement.remove();
}

window.downloadFileFromStream = async (filename, streamReference) => {
    const arrayBuffer = await streamReference.arrayBuffer();
    const blob = new Blob([arrayBuffer]);
    const url = URL.createObjectURL(blob);
    const anchorElement = document.createElement("a");

    anchorElement.href = url;
    anchorElement.download = filename ?? "";
    anchorElement.click();
    anchorElement.remove();
    URL.revokeObjectURL(url);
}
