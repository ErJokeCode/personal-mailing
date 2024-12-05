import * as dashboard from './dashboard/+page';
import http from '../../utility/http'
import { admin } from "../../utility/store.js";

/** @type {import('../(sidebar)/dashboard/$types').PageLoad} */
export function load(request) {
	return dashboard.load(request);
}



export const _hasAdmin = async () => {
	let the_admin = await http.get("/core/admin/me", http.status());
	if (the_admin) {
		return true
	}
	return false
}