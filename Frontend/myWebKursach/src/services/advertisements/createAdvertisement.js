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

export const getCreateAdvertisementFormModel = async (id) => {
  try {
    console.log("Запрос на получение формы создания объявления...");
    console.log("ID объявления:", id);

    const token = localStorage.getItem("authToken");
    if (!token) {
      return { success: false, error: "Токен отсутствует, авторизуйтесь снова." };
    }

    const response = await axios.post(
      `https://localhost:7069/api/Advertisement/CreateAdvertisementModel?id=${id}`,
      {}, // Пустое тело, так как метод не требует данных в теле запроса
      {
        headers: {
          Authorization: `Bearer ${token}`,
          accept: "*/*",
          "Content-Type": "application/json",
        },
        withCredentials: true,
      }
    );

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
      if (error.response) {
          if (error.response.status === 400 && error.response.data.error) {
              return { success: false, errors: [error.response.data.error] };
          }
          if (error.response.data && error.response.data.errors) {
              return { success: false, errors: error.response.data.errors };
          }
      }
      return { success: false, errors: [error.response.data]};
  }
    
};

