import http from 'k6/http';
import { check, sleep } from 'k6';

// Опции теста
export let options = {
    vus: 50, // Количество виртуальных пользователей
    duration: '10s', // Продолжительность теста
};

// Базовый URL для API
const BASE_URL = 'https://localhost:7069/api';

// Функция для генерации случайных данных
function generateRandomUser(index) {
    return {
        password: `pass${index}`,
        email: `user${index}@example.com`,
        role: Math.floor(Math.random() * 2), // 0 или 1
        name: `User${index}`,
        status: Math.random() < 0.5, // true или false
        rating: Math.floor(Math.random() * 5) + 1, // Рейтинг от 1 до 5
        phoneNumber: `+7${Math.floor(1000000000 + Math.random() * 9000000000)}`,
        dateOfRegistration: new Date().toISOString(),
    };
}

function generateRandomAdvertisement(userId) {
    return {
        objectType: Math.floor(Math.random() * 3), // Пример enum: 0, 1, 2
        adressCoordinates: { type: 'Point', coordinates: [37.618423, 55.751244] }, // Координаты
        adressName: `Random Address ${Math.floor(Math.random() * 1000)}`,
        placementStatus: true,
        deletionStatus: false,
        description: `Random description ${Math.floor(Math.random() * 1000)}`,
        typeOfRental: Math.floor(Math.random() * 2), // 0 или 1
        totalArea: Math.floor(Math.random() * 200) + 50,
        rentalPrice: Math.random() * 1000 + 100,
        fixedPrepaymentAmount: Math.random() * 500 + 50,
        numberOfRooms: Math.floor(Math.random() * 5) + 1,
        numberOfBeds: Math.floor(Math.random() * 5) + 1,
        numberOfBathrooms: Math.floor(Math.random() * 3) + 1,
        dateCreate: new Date().toISOString(),
        rating: Math.random() * 5,
        numberOfPromotionPoints: Math.floor(Math.random() * 100),
        idAuthor: userId,
    };
}

function generateRandomRentalRequest(adId, userId) {
    return {
        approvalStatus: Math.floor(Math.random() * 3), // Enum: 0, 1, 2
        deleteStatus: false,
        bookingStartDate: new Date().toISOString(),
        bookingFinishDate: new Date(Date.now() + Math.random() * 100000000).toISOString(),
        recipientsViewingStatus: false,
        authorsViewingStatus: false,
        dataChangeStatus: new Date().toISOString(),
        idAuthorRentalRequest: userId,
        idNeedAdvertisement: adId,
    };
}

function generateRandomReview(rentalRequestId, isLandlord) {
    return {
        theQualityOfTheTransaction: Math.floor(Math.random() * 5) + 1,
        comment: `Random comment ${Math.floor(Math.random() * 1000)}`,
        statusDel: false,
        isTheLandlord: isLandlord,
        recipientsViewingStatus: false,
        authorsViewingStatus: false,
        dateOfCreation: new Date().toISOString(),
        idNeedRentalRequest: rentalRequestId,
    };
}

// Helper function to perform a POST request and check the response
function postRequest(url, payload) {
    const res = http.post(url, JSON.stringify(payload), {
        headers: { 'Content-Type': 'application/json' },
    });

    check(res, {
        'Request status was 200': (r) => r.status === 200,
    });

    return res;
}

export default function () {
    // Шаг 1: Создание пользователей
    let userPayload = [];
    for (let i = 0; i < 5; i++) {
        userPayload.push(generateRandomUser(i));
    }

    let userRes = postRequest(`${BASE_URL}/User/CreateUsersEntity`, userPayload);
    let users = [];
    if (userRes.status === 200 && userRes.body) {
        try {
            users = JSON.parse(userRes.body);
        } catch (error) {
            console.error(`Error parsing user response: ${error.message}`);
        }
    } else {
        console.error(`User creation failed with status: ${userRes.status}`);
    }

    // Шаг 2: Создание объявлений для каждого пользователя
    users.forEach((user) => {
        if (!user.id) {
            console.error(`User with no id: ${JSON.stringify(user)}`);
            return;
        }

        let adPayload = generateRandomAdvertisement(user.id);
        let adRes = postRequest(`${BASE_URL}/Advertisement/CreateAdvertisementEntity`, adPayload);
        let ad = adRes.status === 200 ? JSON.parse(adRes.body) : null;

        if (!ad || !ad.id) {
            console.error(`Advertisement creation failed for user ${user.id}`);
            return;
        }

        // Шаг 3: Создание заявки на аренду
        let rentalRequestPayload = generateRandomRentalRequest(ad.id, user.id);
        let rentalRequestRes = postRequest(`${BASE_URL}/RentalRequest/CreateRentalRequest`, rentalRequestPayload);
        let rentalRequest = rentalRequestRes.status === 200 ? JSON.parse(rentalRequestRes.body) : null;

        if (!rentalRequest || !rentalRequest.id) {
            console.error(`Rental request creation failed for advertisement ${ad.id}`);
            return;
        }

        // Шаг 4: Создание отзывов
        let reviewPayload = generateRandomReview(rentalRequest.id, true);
        postRequest(`${BASE_URL}/Review/CreateReview`, reviewPayload);
    });

    sleep(1); // Задержка между запросами
}
