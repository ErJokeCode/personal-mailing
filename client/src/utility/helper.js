export function isEmptyObject(obj) {
    return JSON.stringify(obj) === "{}";
}

export function isObject(obj) {
    return typeof obj === 'object' && !Array.isArray(obj) && obj !== null;
}

export function traverseObject(obj) {
    let ul = document.createElement("ul");

    for (let key of Object.keys(obj)) {
        if (isObject(obj[key])) {
            let li = document.createElement("li");
            li.textContent = `${key}: `;
            ul.appendChild(li);

            let sub = traverseObject(obj[key])
            ul.appendChild(sub);
        }

        else if (Array.isArray(obj[key])) {
            let li = document.createElement("li");
            li.textContent = `${key}: `;
            ul.appendChild(li);

            let arr = document.createElement("ul");

            for (let item of obj[key]) {
                let el = traverseObject(item);
                arr.appendChild(el);
            }

            ul.appendChild(arr);
        }

        else {
            let li = document.createElement("li");
            li.textContent = `${key}: ${obj[key]}`;
            ul.appendChild(li);
        }
    }

    console.log(ul);

    return ul;
}
