'use strict';

if (typeof Readability !== 'undefined') {
    var doc = document.cloneNode(true);
    var reader = new Readability(doc)
    var parsedDocument = reader.parse();
    var payload = JSON.stringify(
        {
            'url': document.URL,
            'title': document.title,
            'content': parsedDocument.content
        }
    );
    console.log(payload)

    fetch('http://localhost:5000/api/browser',
        {
            method: 'POST',
            mode: 'cors',
            credentials: 'same-origin',
            headers: {
                'Content-Type': 'application/json'
            },
            referrerPolicy: 'no-referrer',
            body: payload,
        })
}
