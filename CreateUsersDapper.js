import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
    vus: 50,
    duration: '10s',
};

export default function () {
    let payload = JSON.stringify([
        {
            password: "pass12",
            email: "user12@example.com",
            role: 0,
            name: "User12",
            status: true,
            rating: 4,
            phoneNumber: "123456789",
            dateOfRegistration: "2024-12-08T00:00:00Z"
        },
        {
            password: "pass13",
            email: "user13@example.com",
            role: 1,
            name: "User13",
            status: false,
            rating: 3,
            phoneNumber: "987654321",
            dateOfRegistration: "2024-12-08T00:00:00Z"
        },
    ]);
    let params = { headers: { "Content-Type": "application/json" } };
    let res = http.post('https://localhost:7069/api/Users/CreateUsersDapper', payload, params);
    check(res, {
        'status was 200': (r) => r.status == 200,
        'response body contains success message': (r) => r.body.includes("Users created successfully.")
    });
    sleep(1);
}