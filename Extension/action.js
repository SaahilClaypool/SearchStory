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
        }).then(() => 
        {
            let id = "search_story_popup";
            let popup = document.getElementById(id) || document.createElement("div", { id })
            popup.id = id;
            try {
                document.body.removeChild(popup)
            } catch {
                console.log('new element')
            }
            // clear
            while(popup.firstChild) popup.removeChild(popup.firstChild);
            let rounded = "border-radius: .5rem;"
            let shadow = "--tw-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04); box-shadow: var(--tw-ring-offset-shadow, 0 0 #0000), var(--tw-ring-shadow, 0 0 #0000), var(--tw-shadow);"
            let center = "display: grid; place-items: center; position: fixed; top: 50%; left: 50%; transform: translate(-50%, -50%); height: 10ch; width: 40ch; text-align: center;"
            let blue = "background-color: lightblue;";
            popup.style = rounded + shadow + center + blue
            popup.appendChild(document.createTextNode("successfully added to Search Story!"));
            document.body.appendChild(popup)
            window.setTimeout(() => document.body.removeChild(popup), 1000);
        })
}
