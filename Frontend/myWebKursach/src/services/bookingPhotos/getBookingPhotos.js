import axios from "axios";

export const getBookingPhotos = async (rentalRequestId, isBefore = null) => {
  try {
    const token = localStorage.getItem("authToken");
    if (!token) {
      return {
        success: false,
        error: "Токен отсутствует. Пожалуйста, авторизуйтесь.",
        data: [],
      };
    }

    const config = {
      headers: {
        Authorization: `Bearer ${token}`,
        Accept: "*/*",
        "Content-Type": "application/json",
      },
      withCredentials: true,
      params: {
        rentalRequestId,
      },
    };

    if (isBefore !== null) {
      config.params.isBefore = isBefore;
    }

    const response = await axios.get(`${API_BASE_URL}/api/BookingPhoto/get`, config);

    return { success: true, data: response.data };
  } catch (error) {
    console.error("Ошибка при получении фотографий бронирования:", error);

    return {
      success: false,
      error: error.response?.data?.error || "Неизвестная ошибка",
      data: [],
    };
  }
};