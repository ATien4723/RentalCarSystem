const validExtensions = ['image/jpeg', 'image/png', 'application/pdf', 'application/msword', 'application/vnd.openxmlformats-officedocument.wordprocessingml.document'];

const areas = [
    { dropArea: '#drag-area4', ahref: '#selectfile4', input: '#fileInput4', inputName: 'front', dragText: '.dragdrop4', imgId: 'frontImg' },
    { dropArea: '#drag-area5', ahref: '#selectfile5', input: '#fileInput5', inputName: 'back', dragText: '.dragdrop5', imgId: 'backImg' },
    { dropArea: '#drag-area6', ahref: '#selectfile6', input: '#fileInput6', inputName: 'left', dragText: '.dragdrop6', imgId: 'leftImg' },
    { dropArea: '#drag-area7', ahref: '#selectfile7', input: '#fileInput7', inputName: 'right', dragText: '.dragdrop7', imgId: 'rightImg' }
];

function showFile(dropArea, file, inputName, imgId, ahref) {
    if (validExtensions.includes(file.type)) {
        const fileReader = new FileReader();
        fileReader.onload = () => {
            const fileUrl = fileReader.result;
            let imgTag = '';

            if (file.type.startsWith('image/')) {
                imgTag = `<img src="${fileUrl}" alt="Selected file" style="max-width: 100%; height: auto;">`;
            } else {
                imgTag = `<p>Selected file: ${file.name}</p>`;
            }
            const ahreff = document.createElement('a');
            ahreff.id = ahref;
            ahreff.href = '#';
            ahreff.textContent = 'Choose another file';
            ahreff.addEventListener('click', (event) => {
                event.preventDefault();
                hiddenInput.click();
            });
            const hiddenInput = document.createElement('input');
            hiddenInput.type = 'file';
            hiddenInput.name = inputName;
            hiddenInput.style.display = 'none';
            hiddenInput.files = fileListWithFile(file);
            hiddenInput.id = imgId;
            hiddenInput.addEventListener('change', (event) => {
                const filee = event.target.files[0];

                if (filee) {
                    showFile(dropArea, filee, inputName, imgId, ahref);
                }
            });
            dropArea.innerHTML = imgTag;
            dropArea.appendChild(ahreff);
            dropArea.appendChild(hiddenInput);
        }
        fileReader.readAsDataURL(file);
    } else {
        alert("File type is illegal");
    }
}

function fileListWithFile(file) {
    const dataTransfer = new DataTransfer();
    dataTransfer.items.add(file);
    return dataTransfer.files;
}

areas.forEach(area => {
    const dropArea = document.querySelector(area.dropArea);
    const ahref = document.querySelector(area.ahref);
    const input = document.querySelector(area.input);
    const dragdrop = document.querySelector(area.dragText);
    const imgId = area.imgId;

    ahref.addEventListener('click', (event) => {
        event.preventDefault();
        input.click();
    });

    input.addEventListener('change', (event) => {
        const file = event.target.files[0];

        if (file) {
            showFile(dropArea, file, area.inputName, imgId, ahref);
        }
    });

    dropArea.addEventListener('dragover', (event) => {
        event.preventDefault();
        dragdrop.textContent = "Drop";
    });

    dropArea.addEventListener('dragleave', (event) => {
        event.preventDefault();
        dragdrop.textContent = "Drag and Drop";
    });

    dropArea.addEventListener('drop', (event) => {
        event.preventDefault();
        dragdrop.textContent = "Drag and Drop";
        const file = event.dataTransfer.files[0];
        if (file) {
            showFile(dropArea, file, area.inputName, imgId, ahref);
        }
    });
});
