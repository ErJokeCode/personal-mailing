import http from '../utils/http.js';
import { admin } from "../utils/store.js";

export const _hasAdmin = async () => {
    let the_admin = await http.get("/core/admin/me", http.status());
    if (the_admin) {
        return true
    }
    return false
}
