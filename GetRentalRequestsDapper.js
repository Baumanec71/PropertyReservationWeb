import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
    vus: 200,
    duration: '30s',
};

export default function () {
    const page = 1;
    const pageSize = 1000;
    let res = http.get(`https://localhost:7069/api/RentalRequest/GetRentalRequestsDapper?page=${page}&pageSize=${pageSize}`);

    check(res, {
        'ответ содержит rentalRequests': (r) => {
            let json = r.json();
            return json.rentalRequests !== undefined && Array.isArray(json.rentalRequests);
        },
    });
    sleep(1);
}