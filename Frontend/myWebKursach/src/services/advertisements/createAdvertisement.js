import axios from "axios";

export const getCreateAdvertisementForm = async () => {
  try {
    const token = localStorage.getItem("authToken");
    if (!token) {
      return { success: false, error: "Токен отсутствует, авторизуйтесь снова." };
    }

    const response = await axios.get("https://localhost:7069/api/Advertisement/CreateAdvertisement", {
      headers: {
        Authorization: `Bearer ${token}`,
        accept: "*/*"
      },
      withCredentials: true,
    });

    return { success: true, data: response.data };
  } catch (error) {
    console.error("Ошибка при получении формы создания объявления:", error);
    return { success: false, error: error.response?.data?.error || "Неизвестная ошибка" };
  }
};

export const createAdvertisement = async (advertisementData) => {
    try {
        const token = localStorage.getItem("authToken");
        if (!token) {
            return { success: false, error: "Токен отсутствует, авторизуйтесь снова." };
        }

        const response = await axios.post("https://localhost:7069/api/Advertisement/CreateAdvertisement", advertisementData, {
            headers: {
                Authorization: `Bearer ${token}`,
                Accept: "*/*",
                "Content-Type": "application/json",
            },
            withCredentials: true,
        });

        return { success: true, data: response.data };
    } catch (error) {
      console.log(error)
      if (error.response) {
          if (error.response.status === 400 && error.response.data.error) {
              // Обрабатываем ошибку BadRequest (400)
             // console.log(error.response.data.error);
              return { success: false, errors: [error.response.data.error] };
          }
          if (error.response.data && error.response.data.errors) {
         //   console.log(error.response.data.errors);
              // Обрабатываем другие ошибки
              return { success: false, errors: error.response.data.errors };
          }
      }
      // В случае других ошибок сервера
      return { success: false, errors: [error.response.data]};
  }
    
};

