import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
    vus: 200,
    duration: '30s',
};

export default function () {
    let res = http.get('https://localhost:7069/api/RentalRequest/GetRentalRequestsAdoNet');
    check(res, {
        'status was 200': (r) => r.status === 200
    });
    sleep(1);
}