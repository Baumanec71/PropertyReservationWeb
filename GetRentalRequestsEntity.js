import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
    vus: 1, // Количество виртуальных пользователей
    duration: '1s', // Длительность теста
};

export default function () {
    // Указываем параметры запроса
    const page = 1;
    const pageSize = 1000;

    // Отправляем GET-запрос с query-параметрами
    let res = http.get(`https://localhost:7069/api/RentalRequest/GetRentalRequestsEntity?page=${page}&pageSize=${pageSize}`);

    // Проверка ответа
    check(res, {
     /*   'статус 200': (r) => r.status === 200, // Успешный статус*/
        'ответ содержит rentalRequests': (r) => {
            let json = r.json();
            return json.rentalRequests !== undefined && Array.isArray(json.rentalRequests);
        },
        //'ответ содержит totalRecords': (r) => {
        //    let json = r.json();
        //    return json.totalRecords !== undefined && json.totalRecords >= 0;
        //},
        //'общее количество страниц корректно': (r) => {
        //    let json = r.json();
        //    return json.totalPages >= 1;
        //},
    });

    // Пауза для снижения нагрузки
    sleep(1);
}