import http from 'k6/http';
import { check, sleep } from 'k6';

function randomDate(start, end) {
    return new Date(start.getTime() + Math.random() * (end.getTime() - start.getTime())).toISOString();
}

function randomBigInt(min, max) {
    return Math.floor(Math.random() * (max - min + 1)) + min;
}

export let options = {
    vus: 200,
    duration: '30s',
};
function randomInt(min, max) {
    return Math.floor(Math.random() * (max - min + 1)) + min;
}
export default function () {
    const url = 'https://localhost:7069/api/RentalRequest/CreateRentalRequestAdoNet';
    const payload = JSON.stringify({
        approvalStatus: randomInt(1, 3),
        deleteStatus: Math.random() > 0.5,
        bookingStartDate: randomDate(new Date('2024-01-01'), new Date('2024-12-31')),
        bookingFinishDate: randomDate(new Date('2024-01-02'), new Date('2025-01-01')),
        recipientsViewingStatus: Math.random() > 0.5,
        authorsViewingStatus: Math.random() > 0.5,
        dataChangeStatus: randomDate(new Date('2023-12-01'), new Date('2024-12-31')),
        idAuthorRentalRequest: randomBigInt(224188, 225987),
        idNeedAdvertisement: randomBigInt(15696, 17495),
    });

    const params = {
        headers: {
            'Content-Type': 'application/json',
        },
    };

    const res = http.post(url, payload, params);

    check(res, {
        'status is 200': (r) => r.status === 200,
        'response time < 500ms': (r) => r.timings.duration < 500,
    });

    sleep(1);
}