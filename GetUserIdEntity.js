import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
    vus: 1, // Количество виртуальных пользователей
    duration: '10s', // Длительность теста
};

export default function () {
    // Тестирование GetUserIdEntity с примером id
    let id = 224544;  // Пример ID пользователя
    let res1 = http.get(`https://localhost:7069/api/User/GetUserIdEntity?id=${id}`);
    //let res1 = http.get(`https://localhost:7069/api/User/GetUserIdEntity?id=${id}`, {
    //    timeout: '60s', // Устанавливаем таймаут запроса
    //});
    check(res1, {
        'status was 200 for GetUserIdEntity': (r) => r.status == 200
      /*  'Response contains user id': (r) => r.body && r.body.includes(id.toString())*/
    });// Простая проверка

    // Задержка между запросами для имитации более реального поведения пользователей
    sleep(1);
}