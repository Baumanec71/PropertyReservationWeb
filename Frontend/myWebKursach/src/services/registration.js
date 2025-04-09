import axios from "axios";

export const registration = async (email, password, passwordConfirm) => {
    try {
        const response = await axios.post(`${API_BASE_URL}/api/Auth/register`, {
            email,
            password,
            passwordConfirm
        });

        return { success: true, data: response }; // Успешная регистрация
    } catch (error) {
        if (error.response) {
            if (error.response.status === 400 && error.response.data.error) {
                // Обрабатываем ошибку BadRequest (400)
                return { success: false, errors: [error.response.data.error] };
            }
            if (error.response.data && error.response.data.errors) {
                // Обрабатываем другие ошибки
                return { success: false, errors: error.response.data.errors };
            }
        }
        // В случае других ошибок сервера
        return { success: false, errors: ["Ошибка сервера. Попробуйте позже."] };
    }
};