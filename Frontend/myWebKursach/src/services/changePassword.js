import axios from "axios";

export const changePassword = async (oldPassword, newPassword, newPasswordConfirm) => {   //email, 
    try {
        const token = localStorage.getItem("authToken");
        const response = await axios.post("https://localhost:7069/api/Auth/change-password", {
           // email,
            oldPassword,
            newPassword,
            newPasswordConfirm
        },
        {
            withCredentials: true,
            headers: {
              // Передаём токен в заголовке Authorization
              Authorization: `Bearer ${token}`,
            },
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
