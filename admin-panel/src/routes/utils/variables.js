export const imgDir = '';

/** @type {(x:string) => string} */
export const avatarPath = (src) => imgDir + '/users/' + src;

/** @type {(x:string, ...y:string[]) => string} */
export const imagesPath = (src, ...subdirs) => [imgDir, ...subdirs, src].filter(Boolean).join('/');

import http from '../../utility/http'
import { admin } from "../../utility/store.js";

http.get("/core/admin/me", http.status()).then((value) => {
    if (value) {
        admin.update((_) => value);
    }
})
