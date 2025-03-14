import axios from "axios";

export const getAdvertisements = async (page = 1, filterModel = null) => {
    try {
        const token = localStorage.getItem("authToken");
        if (!token) {
            return { success: false, errors: ["Токен отсутствует, авторизуйтесь снова."] };
        }

        const config = {
            headers: {
                Authorization: `Bearer ${token}`,
                Accept: "application/json",
                "Content-Type": "application/json",
            },
            withCredentials: true,
        };

        let response;

        if (!filterModel) {
            // GET-запрос, если нет фильтров
            console.log("Get - запрос:");
            response = await axios.get(
                `https://localhost:7069/api/Advertisement/GetAdvertisements?page=${page}`,
                config
            );
        } else {
            // POST-запрос, если фильтры переданы
            console.log("Post - запрос:");
            response = await axios.post(
                `https://localhost:7069/api/Advertisement/GetAdvertisements?page=${page}`,
                filterModel,
                config
            );
        }

        return response.data;
    } catch (error) {
        console.error("Ошибка при получении объявлений:", error);
        return { viewModels: [], totalPages: 1 };
    }
};

{/*import axios from "axios";

export const getAdvertisements = async (page = 1, filterModel) => {
    try {
      const token = localStorage.getItem("authToken");
      if (!token) {
        return { success: false, errors: ["Токен отсутствует, авторизуйтесь снова."] };
      }
      console.log(filterModel.createAdvertisementAmenities);
      // Передаём filterModel, оставляя selectedAmenities без преобразования
      const transformedFilter = {
        ...filterModel,
      };
  
      const params = { page, ...transformedFilter };
  
      const response = await axios.get("https://localhost:7069/api/Advertisement/GetAdvertisements", {
        params,
        headers: {
          Authorization: `Bearer ${token}`,
          Accept: "application/json",
          "Content-Type": "application/json",
        },
        withCredentials: true,
      });
  
      return response.data;
    } catch (error) {
      console.error("Ошибка при получении объявлений:", error);
      return { viewModels: [], totalPages: 1 };
    }
  }; */}