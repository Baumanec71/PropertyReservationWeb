import axios from "axios";

export const getPaymentRentalRequest = async (id) => {
    try {
        const token = localStorage.getItem("authToken");
        if (!token) {
            return { success: false, error: "Токен отсутствует, авторизуйтесь снова." };
        }

        const response = await axios.post(
            `${API_BASE_URL}/api/PaymentRentalRequest/GetPaymentRentalRequest`, 
            null, // Тело запроса не передаем, так как id идет в query
            {
                params: { id },
                headers: {
                    Authorization: `Bearer ${token}`,
                    Accept: "*/*",
                    "Content-Type": "application/json",
                },
                withCredentials: true,
            }
        );

        if (response.status === 200) {
            window.location.href = response.request.responseURL;
            return { success: true };
        }

        return { success: false, error: "Не удалось получить ссылку на оплату." };

    } catch (error) {
        if (error.response) {
            return { success: false, error: error.response.data || "Ошибка при запросе" };
        }
        return { success: false, error: "Сетевая ошибка" };
    }
};