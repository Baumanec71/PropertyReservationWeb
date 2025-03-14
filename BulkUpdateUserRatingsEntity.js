import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
    vus: 20,
    duration: '30s',
};

export default function () {
    let url = 'https://localhost:7069/api/User/BulkUpdateUserRatingsEntity';

    // Параметры для запроса
    let params = {
        headers: { "Content-Type": "application/json" }
    };

    // Тело запроса с параметрами minRating и increment
    let payload = JSON.stringify({
        minRating: 4,
        increment: -1
    });

    // Отправка PUT запроса
    let res = http.put(url, payload, params);

    // Проверки для ответа
    check(res, {
        'status was 200': (r) => r.status == 200,
        'response body contains success message': (r) => r.body.includes("Ratings updated successfully.")
    });

    sleep(1); // Задержка между запросами
}