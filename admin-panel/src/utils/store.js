import { writable } from 'svelte/store';

export const server_url = "http://localhost:5000";

export const admin = writable({});

export const notifications = [];
