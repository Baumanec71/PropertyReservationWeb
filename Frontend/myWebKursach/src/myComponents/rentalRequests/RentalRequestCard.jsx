import { Card, Stack, Text, Button, Box, Flex, HStack, VStack, } from "@chakra-ui/react";
import { useState, useEffect } from "react";
import { getUserRole } from "../../services/InfoJwt/getUserRole";
import { getUserEmail } from "../../services/InfoJwt/getUserEmail";
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";
import { registerLocale, setDefaultLocale } from "react-datepicker";
import ru from "date-fns/locale/ru"; // Импортируем русскую локализацию
registerLocale("ru", ru);
setDefaultLocale("ru");
import { parse } from 'date-fns';
import { getUserId } from "../../services/InfoJwt/getUserId";
import axios from "axios";
import { useNavigate } from "react-router-dom";
import { getPaymentRentalRequest } from "@/services/payments/getPaymentRentalRequest";



export default function RentalRequestCard({ request, onEdit }) {
  const [role, setRole] = useState(null);
  const [email, setEmail] = useState(null);
  const [loading, setLoading] = useState(false);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [id, setId] = useState(null);
  const [reload, setReload] = useState(false);
  const navigate = useNavigate();

  const handlePaymentRequest = async (idpayment) => {
    const token = localStorage.getItem("authToken");
    if (!token) {
      return { success: false, error: "Токен отсутствует, авторизуйтесь снова." };
    }
    const response = await axios.post(`${API_BASE_URL}/api/PaymentRentalRequest/GetPaymentRentalRequest?id=${idpayment}`, {}, {
      headers: {
          Authorization: `Bearer ${token}`,
      },
      withCredentials: true,
  });
  
  if (response.data.paymentUrl) {
      window.location.href = response.data.paymentUrl;
  }
};

const handleApprove = async (id) => {
  try {
    const token = localStorage.getItem("authToken");
    if (!token) return;

    const response = await axios.put(
      `${API_BASE_URL}/api/RentalRequest/CreateApprovalStatusTrueAdvertisementForUser?id=${id}`,
      {},
      {
        headers: {
          Authorization: `Bearer ${token}`,
          'accept': '*/*',
          'Content-Type': 'application/json'
        },
        withCredentials: true,
      }
    );

    if (onEdit) onEdit(); // Обновляем родительский список или состояние
  } catch (error) {
    console.error("Ошибка при одобрении запроса:", error);
  }
};

const handleReject = async (id) => {
  try {
    const token = localStorage.getItem("authToken");
    if (!token) return;

    const response = await axios.put(
      `${API_BASE_URL}/api/RentalRequest/CreateApprovalStatusFalseAdvertisementForUser?id=${id}`,
      {},
      {
        headers: {
          Authorization: `Bearer ${token}`,
          'accept': '*/*',
          'Content-Type': 'application/json'
        },
        withCredentials: true,
      }
    );

    if (onEdit) onEdit(); // Аналогично
  } catch (error) {
    console.error("Ошибка при отклонении запроса:", error);
  }
};

const handleDelete = async (id) => {
  try {
    const token = localStorage.getItem("authToken");
    if (!token) return;

    const response = await axios.put(
      `${API_BASE_URL}/api/RentalRequest/DeleteRentalRequestForUser?id=${id}`,
      {},
      {
        headers: {
          Authorization: `Bearer ${token}`,
          'accept': '*/*',
          'Content-Type': 'application/json'
        },
        withCredentials: true,
      }
    );

    if (onEdit) onEdit(); // Удаляем элемент без перезагрузки страницы
  } catch (error) {
    console.error("Ошибка при удалении запроса:", error);
  }
};
  useEffect(() => {
    const checkAuth = () => {
      
      const token = localStorage.getItem("authToken");
      setIsAuthenticated(!!token);
      if (token) {
        setId(getUserId());
        setRole(getUserRole());
        setEmail(getUserEmail());
      } else {
        setRole(null);
        setEmail(null);
      }
    };

    checkAuth();
  }, []);

  const parseDate = (dateString) => {
    return parse(dateString, 'dd-MM-yyyy', new Date());
  };
  
  const startDate = parseDate(request.bookingStartDate);
  const endDate = parseDate(request.bookingEndDate);

    // Функция для расчета количества дней между датами
    const getDaysBetween = (start, end) => {
      const oneDay = 24 * 60 * 60 * 1000;
      const diffTime = end.getTime() - start.getTime();
      const diffDays = Math.ceil(diffTime / oneDay);
      return diffDays+1;
    };
  
    const daysBetween = getDaysBetween(startDate, endDate);

  return (
    <Card.Root p={5} boxShadow="lg" borderRadius="lg" bg="white" border="1px" borderColor="gray.200">
      <Card.Header>
        <Text fontSize="2xl" fontWeight="bold" color="teal.600">
          Заявка на аренду #{request.id} на {daysBetween} дней
          на {request.price} ₽
        </Text>
      </Card.Header>

      <Card.Body>
        <Stack spacing={4}>
        <Flex justify="space-between" align="center"> 
  <Text fontWeight="bold" color={
    request.approvalStatus === "Одобрен" ? "green.500" :
    request.approvalStatus === "Оплачен" ? "green.500" :
    request.approvalStatus === "Завершен успешно" ? "green.500" :
    request.approvalStatus === "Отклонен" ? "red.500" :
    "black"
  }>
    <b>{request.approvalStatus}</b>
  </Text>
</Flex>

<Flex justify="center">
  <Box 
    maxWidth="100%" 
    overflowX="auto" 
    minWidth="250px" 
    borderRadius="md" 
  >
    <DatePicker
      selected={startDate}
      startDate={startDate}
      endDate={endDate}
      selectsRange
      inline
      disabled  
    />
  </Box>
</Flex>

          {request.dataChangeStatus && request.dataChangeStatus !== "01-01-0001" && (
  <Text fontWeight="bold" color="gray.600">
    <b>Дата изменения статуса: </b> {request.dataChangeStatus}
  </Text>
)}
          <Text fontWeight="bold" color="gray.600"><b>ID автора заявки:</b> {request.idAuthorRentalRequest}</Text>
          <Text fontWeight="bold" color="gray.600"><b>ID объявления:</b> {request.idNeedAdvertisement}</Text>
          <Text fontWeight="bold" color="gray.600"><b>ID платежа:</b> {request.paymentId ?? "Не указан"}</Text>
        </Stack>
      </Card.Body>

      <Card.Footer justifyContent="flex-end" mt={4}>
      {isAuthenticated && (
  <Box w="100%">
    <Stack mt={4} spacing={4}>
      {/* Кнопка удаления для админа */}
      {role === "Admin" && (
        <Button
          variant="solid"
          colorScheme="red"
          w="100%"
          size="lg"
          _hover={{ bg: "red.600" }}
          onClick={() => {/* Delete logic */}}
        >
          Удалить запрос
        </Button>
      )}
  {request.approvalStatus === "Завершен успешно" && (
  
      <HStack spacing={4} mt={2}>
              <Button 
      size="lg"
      colorScheme="blue"
      w="full"
      onClick={() => navigate(`/createReview/${request.id}`)}
      >
        Оставить отзыв
        </Button>
      </HStack>
    )}
      {/* Кнопки для автора объявления */}
      {id?.toString() === request.idAuthorAdvertisement?.toString() && (
        <>
          <HStack spacing={4}>
            <Button
              variant="solid"
              colorScheme="green"
              w="50%"
              size="md"
              _hover={{ bg: "green.600" }}
              onClick={() => handleApprove(request.id)}
            >
              Одобрить запрос
            </Button>
            <Button
              variant="solid"
              colorScheme="red"
              w="50%"
              size="md"
              _hover={{ bg: "red.600" }}
              onClick={() => handleReject(request.id)}
            >
              Отклонить запрос
            </Button>
          </HStack>
          <HStack spacing={4}>
            <Button
              variant="solid"
              colorScheme="blue"
              w="100%"
              size="md"
              _hover={{ bg: "blue.600" }}
              onClick={async () => {
                try {
                  navigate(`/user/${request.idAuthorRentalRequest}`);
                } catch (error) {
                  console.error("Ошибка при обработке запроса:", error);
                }
              }}
            >
              Автор запроса
            </Button>
          </HStack>
        </>
      )}

      {/* Кнопки для автора запроса аренды */}
      {id?.toString() === request.idAuthorRentalRequest?.toString() && (
  <>
    <HStack spacing={4}>
      <Button
        variant="solid"
        colorScheme="blue"
        w="50%"
        size="md"
        _hover={{ bg: "blue.600" }}
        onClick={async () => {
          try {
            navigate(`/user/${request.idAuthorAdvertisement}`);
          } catch (error) {
            console.error("Ошибка при обработке запроса:", error);
          }
        }}
      >
        Владелец недвижимости
      </Button>
      <Button
        variant="solid"
        colorScheme="red"
        w="50%"
        size="md"
        _hover={{ bg: "red.600" }}
        onClick={async () => {
          try {
            await handleDelete(request.id);
          } catch (error) {
            console.error("Ошибка при удалении запроса:", error);
          }
        }}
      >
        Удалить запрос
      </Button>
    </HStack>
    {request.paymentId != "" && (
      <HStack spacing={4} mt={2}>
        <Button
          variant="solid"
          colorScheme="green"
          w="50%"
          size="md"
          _hover={{ bg: "green.600" }}
          onClick={async () => {
            try {
              await handlePaymentRequest(request.paymentId);
            } catch (error) {
              console.error("Ошибка при обработке платежа:", error);
            }
          }}
        >
          Оплатить запрос
        </Button>
      </HStack>
    )}
  </>
)}
    </Stack>
  </Box>
)}
</Card.Footer>
    </Card.Root>
  );
}