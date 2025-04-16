import axios from "axios";

export const addBookingPhotos = async (photos, rentalRequestId) => {
  try {
    const token = localStorage.getItem("authToken");
    if (!token) {
      return {
        success: false,
        error: "Токен отсутствует. Пожалуйста, авторизуйтесь.",
      };
    }

    const response = await axios.post(
      `${API_BASE_URL}/api/BookingPhoto/add?rentalRequestId=${rentalRequestId}`,
      photos,
      {
        headers: {
          Authorization: `Bearer ${token}`,
          Accept: "*/*",
          "Content-Type": "application/json",
        },
        withCredentials: true,
      }
    );

    return { success: true, data: response.data };
  } catch (error) {
    console.error("Ошибка при добавлении фотографий:", error);

    return {
      success: false,
      error:
        error.response?.data?.error ||
        error.response?.data ||
        "Неизвестная ошибка",
    };
  }
};