import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
    vus: 1, // Количество виртуальных пользователей
    duration: '10s', // Длительность теста
};

export default function () {
    let id = 224544;  // Пример ID пользователя

    // Тестирование GetUserIdDapper с примером id
    let res3 = http.get(`https://localhost:7069/api/User/GetUserIdDapper?id=${id}`);
    check(res3, { 'status was 200 for GetUserIdDapper': (r) => r.status == 200 });


    // Задержка между запросами для имитации более реального поведения пользователей
    sleep(1);
}