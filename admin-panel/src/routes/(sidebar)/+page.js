import http from '../utils/http.js';

export const _hasAdmin = async () => {
    let the_admin = await http.get("/core/admin/me", http.status());
    if (the_admin) {
        return true
    }
    return false
}
