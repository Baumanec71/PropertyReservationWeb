import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
    vus: 200,
    duration: '30s',
};

export default function () {
    let res = http.get('https://localhost:7069/api/User/GetUsersDapper');
    check(res, {
        'ответ содержит данные': (r) => r.json().users.length > 0 || r.json().users.length === 0, 
    });
    sleep(1);
}