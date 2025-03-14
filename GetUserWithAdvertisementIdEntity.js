import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
    vus: 200, // Количество виртуальных пользователей
    duration: '30s', // Длительность теста
};

export default function () {
    let id = 224544;  // Пример ID пользователя

    // Тестирование GetUserWithAdvertisementIdEntity с примером id
    let res2 = http.get(`https://localhost:7069/api/User/GetUserWithAdvertisementIdEntity?id=${id}`);
    check(res2, { 'status was 200 for GetUserWithAdvertisementIdEntity': (r) => r.status == 200 });


    // Задержка между запросами для имитации более реального поведения пользователей
    sleep(1);
}