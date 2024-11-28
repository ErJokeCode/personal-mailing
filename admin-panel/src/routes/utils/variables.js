export const imgDir = '';

/** @type {(x:string) => string} */
export const avatarPath = (src) => imgDir + '/users/' + src;

/** @type {(x:string, ...y:string[]) => string} */
export const imagesPath = (src, ...subdirs) => [imgDir, ...subdirs, src].filter(Boolean).join('/');

import http from '../../utility/http'
import { admin } from "../../utility/store.js";

let the_admin = await http.get("/core/admin/me", http.status());
if (the_admin) {
    admin.update((_) => the_admin);
}