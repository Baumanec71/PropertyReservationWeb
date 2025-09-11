import { Card, Stack, Text, Button, Box, Flex, HStack, VStack, Input, Collapsible, Textarea  } from "@chakra-ui/react";
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
import { Field } from "@/components/ui/field";
import { IoWarningSharp } from "react-icons/io5";
import { Tooltip } from "@/components/ui/tooltip"

export default function RentalRequestCard({ request, onEdit }) {
  const [role, setRole] = useState(null);
  const [email, setEmail] = useState(null);
  const [loading, setLoading] = useState(false);
  const [fixedPrepaymentAmount, setFixedPrepaymentAmount] = useState(request.fixedPrepaymentAmount || 0);
  const [fixedDepositAmount, setFixedDepositAmount] = useState(request.fixedDepositAmount || 0);
  const [isPhotoSkippedByLandlord, setIsPhotoSkippedByLandlord] = useState(request.isPhotoSkippedByLandlord || true);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [open, setOpen] = useState(false);
  const [description, setDescription] = useState("");
  const [id, setId] = useState(null);
  const [reload, setReload] = useState(false);
  const navigate = useNavigate();
  const [showComplaintForm, setShowComplaintForm] = useState(false);


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

const handleApprove = async (id, fixedPrepaymentAmount, fixedDepositAmount, isPhotoSkippedByLandlord) => {
  try {
    const token = localStorage.getItem("authToken");
    if (!token) return;

    const response = await axios.put(
      `${API_BASE_URL}/api/RentalRequest/CreateApprovalStatusTrueAdvertisementForUser` +
        `?id=${id}&fixedPrepaymentAmount=${fixedPrepaymentAmount}` +
        `&fixedDepositAmount=${fixedDepositAmount}` +
        `&isPhotoSkippedByLandlord=${isPhotoSkippedByLandlord}`,
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

const handleStart = async (id) => {
  try {
    const token = localStorage.getItem("authToken");
    if (!token) return;
    console.log(id)
    const response = await axios.put(
      `${API_BASE_URL}/api/RentalRequest/CreateApprovalStatusTheBookingHasStartedAdvertisementForUser` +
        `?id=${id}`,
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
    console.log(response)
    alert("Заезд подтвержден")
    if (onEdit) onEdit(); // Обновляем родительский список или состояние
  } catch (error) {
    console.error("Ошибка при обработки въезда:", error);
    alert(error.response.data)
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
    alert(response.data)
    if (onEdit) onEdit(); // Аналогично
  } catch (error) {
    console.error("Ошибка при отклонении запроса:", error);
    alert(error.response.data)
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

    alert(response.data)
    if (onEdit) onEdit(); // Удаляем элемент без перезагрузки страницы
  } catch (error) {
    alert(error.response.data)
    console.error("Ошибка при удалении запроса:", error);
    console.log("data:", error.response.data);
   // alert(response.data)
  }
};

const handleShowForm = () => setShowComplaintForm((prev) => !prev);

const handleLandlordUnhappy = async (id, description) => {
  try {
    setLoading(true);
    const token = localStorage.getItem("authToken");
    if (!token) return;

    const response = await axios.put(
      `${API_BASE_URL}/api/RentalRequest/CreateApprovalStatusTheLandlordIsUnhappyAdvertisementForUser?id=${id}&description=${description}`,
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
    alert(response.data);
    if (onEdit) onEdit(); // Обновляем родительский список или состояние
  } catch (error) {
    console.error("Ошибка при создании статуса 'Недоволен арендодатель':", error);
    alert(error.response?.data || "Ошибка при выполнении запроса.");
  } finally {
    setLoading(false);
  }
};

const handleTenantUnhappy = async (id, description) => {
  try {
    setLoading(true);
    const token = localStorage.getItem("authToken");
    if (!token) return;

    const response = await axios.put(
      `${API_BASE_URL}/api/RentalRequest/CreateApprovalStatusTheTenantIsUnhappyAdvertisementForUser?id=${id}&description=${description}`,
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
    alert(response.data);
    if (onEdit) onEdit(); // Обновляем родительский список или состояние
  } catch (error) {
    console.error("Ошибка при создании статуса 'Недоволен арендатор':", error);
    alert(error.response?.data || "Ошибка при выполнении запроса.");
  } finally {
    setLoading(false);
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
    const isLandlord = id?.toString() === request.idAuthorAdvertisement?.toString();
    const isTenant = id?.toString() === request.idAuthorRentalRequest?.toString();
  
    const handleSubmit = () => {
      if (isLandlord) {
        handleLandlordUnhappy(request.id, description);
      } else if (isTenant) {
        handleTenantUnhappy(request.id, description);
      }
    };

  return (
    <Card.Root p={5} boxShadow="lg" borderRadius="lg" bg="white" border="1px" borderColor="gray.200">
      <Card.Header>
        <Text fontSize="2xl" fontWeight="bold" color="teal.600">
          Заявка на аренду #{request.id} на {daysBetween} дней
          на {request.price} ₽
        </Text>
        {(id?.toString() === request.idAuthorRentalRequest?.toString() &&
        request.isAfterPhotosUploaded === false &&
        request.fixedDepositAmount !== 0) && (
        <Tooltip color = "black" content="Не забудьте добавить фотографии перед выездом" hasArrow placement="top">
          <Button
            px={2}
            py={1}
            fontSize="xs"
            rounded="full"
            w = "15%"
            bg="#FDD835"
            color="black"
            transition="all 0.3s ease"
            _hover={{
              bg: "#FDD835",
              transform: "scale(1.04)",
              color: "black",
              boxShadow: "0 6px 14px rgba(253, 216, 53, 0.35)"
            }}
            _active={{
              transform: "scale(0.98)",
              boxShadow: "0 2px 6px rgba(253, 216, 53, 0.2)"
            }}
            ml={2}
          >
            <IoWarningSharp />
          </Button>
        </Tooltip>
      )}

      {(id?.toString() === request.idAuthorAdvertisement?.toString() &&
        request.isBeforePhotosUploaded === false &&
        request.fixedDepositAmount !== 0) && (
        <Tooltip color = "black" content="Не забудьте добавить фотографии до заезда" hasArrow placement="top">
          <Button
            px={2}
            py={1}
            fontSize="xs"
            w = "15%"
            rounded="full"
            bg="#FDD835"
            color="black"
            transition="all 0.3s ease"
            _hover={{
              bg: "#FDD835",
              transform: "scale(1.04)",
              color: "black",
              boxShadow: "0 6px 14px rgba(253, 216, 53, 0.35)"
            }}
            _active={{
              transform: "scale(0.98)",
              boxShadow: "0 2px 6px rgba(253, 216, 53, 0.2)"
            }}
            ml={2}
          >
            <IoWarningSharp />
          </Button>
        </Tooltip>
      )}
              
      </Card.Header>

      <Card.Body>
        <Stack spacing={4}>
        <Flex justify="space-between" align="center"> 
  <Text fontWeight="bold" color={
    request.approvalStatus === "Одобрен" ? "green.500" :
    request.approvalStatus === "Внесен залог" ? "orange.500" :
    request.approvalStatus === "Внесена предоплата" ? "orange.500" :
    request.approvalStatus === "Даты забронированы" ? "green.500" :
    request.approvalStatus === "Арендатор заехал" ? "green.500" :
    request.approvalStatus === "Завершена успешно" ? "green.500" :
    request.approvalStatus === "Отклонен" ? "red.500" :
    request.approvalStatus === "Участники недовольны" ? "red.500" :
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

          {request.dataChangeStatus && request.dataChangeStatus !== "01:01:0001" && (
  <Text fontWeight="bold" color="gray.600">
    <b>Дата изменения статуса: </b> {request.dataChangeStatus}
  </Text>
)}
         {/* request.dataChangeStatus <Text fontWeight="bold" color="gray.600"><b>ID автора заявки:</b> {request.idAuthorRentalRequest}</Text>
        <Text fontWeight="bold" color="gray.600"><b>ID объявления:</b> {request.idNeedAdvertisement}</Text>*/} 
          <Text fontWeight="bold" color="gray.600"><b>Время заезда:</b> {request.checkInTime}</Text>
          <Text fontWeight="bold" color="gray.600"><b>Время выезда:</b> {request.checkOutTime}</Text>
          {request.fixedPrepaymentAmount != null && (
                <>
                <Text fontWeight="bold" color="gray.600"><b>Сумма предоплаты:</b> {request.fixedPrepaymentAmount ?? "Не указана"}</Text>
                </>
              )}

{request.fixedDepositAmount != 0 && (
                <>
                <Text fontWeight="bold" color="gray.600"><b>Сумма залога:</b> {request.fixedDepositAmount ?? "Залога нет"}</Text>
                </>
              )}
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

        {request.approvalStatus === "Завершена успешно" && (
          <HStack spacing={4} mt={2}>
            <Button
              size="lg"
              bg="black"
              color="white"
              w="full"
              _hover={{ bg: "green" }}
              onClick={() => navigate(`/createReview/${request.id}`)}
            >
              Оставить отзыв
            </Button>
          </HStack>
        )}

        {/* Кнопки для автора объявления */}
        {request.approvalStatus === "Бронь началась" || request.approvalStatus === "Участники недовольны" || request.approvalStatus === "Оплачен" && (
  <Button
    variant="solid"
    bg="black"
    w="50%"
    size="md"
    _hover={{ bg: "blue.600" }}
    onClick={async () => {
      try {
        navigate(`/bookingPhotos/${request.id}`);
      } catch (error) {
        console.error("Ошибка при обработке запроса:", error);
      }
    }}
  >
    Фото до/после
  </Button>
)}
        {id?.toString() === request.idAuthorAdvertisement?.toString() &&
          (request.approvalStatus === "На рассмотрении" || request.approvalStatus === "Отклонен") && (
            <>
              <Field label="Предоплата">
                <Input
                  name="fixedPrepaymentAmount"
                  type="number"
                  value={fixedPrepaymentAmount}
                  onChange={(e) => {
                    let val = parseInt(e.target.value, 10);
                    if (isNaN(val)) val = 0;
                    if (val < 0) val = 0;
                    setFixedPrepaymentAmount(val);
                  }}
                />
              </Field>

              {!isPhotoSkippedByLandlord && (
                <Field label="Залог">
                  <Input
                    name="fixedDepositAmount"
                    type="number"
                    value={fixedDepositAmount}
                    onChange={(e) => {
                      let val = parseInt(e.target.value, 10);
                      if (isNaN(val)) val = 0;
                      if (val < 0) val = 0;
                      setFixedDepositAmount(val);
                    }}
                  />
                </Field>
              )}

              <Box mt={4}>
                <Button
                  bg={isPhotoSkippedByLandlord ? "black" : "blue"}
                  color="white"
                  onClick={() => setIsPhotoSkippedByLandlord((prev) => !prev)}
                  _hover={{ bg: "#FDD835", color: "black" }}
                >
                  {isPhotoSkippedByLandlord
                    ? "Задать залог"
                    : "Залог не нужен"}
                </Button>
              </Box>
            </>
          )}

        {id?.toString() === request.idAuthorAdvertisement?.toString() && (
          <>
            <HStack spacing={4}>
              <Button
                variant="solid"
                bg="black"
                w="50%"
                size="md"
                _hover={{ bg: "green" }}
                onClick={() => handleApprove(request.id, fixedPrepaymentAmount, fixedDepositAmount, isPhotoSkippedByLandlord)}
              >
                Одобрить запрос
              </Button>
              {request.approvalStatus === "Бронь началась" ? (
                <Box w="50%">
                  <Button
                    onClick={handleShowForm}
                    colorScheme="gray"
                    size="md"
                    w="100%"
                  >
                    {showComplaintForm ? "Скрыть" : "Отменить бронь"}
                  </Button>

                  {showComplaintForm && (
                    <Box mt={4}>
                      <VStack align="start" spacing={3}>
                        <Textarea
                          value={description}
                          onChange={(e) => setDescription(e.target.value)}
                          placeholder="Опишите проблему"
                          size="sm"
                        />
                        <Tooltip
                          content={
                            isLandlord
                              ? "Сообщить о недовольстве арендодателя"
                              : "Если вы заехали и хотите отменить бронь"
                          }
                          hasArrow
                          placement="top"
                        >
                          <Button
                            colorScheme="red"
                            isLoading={loading}
                            onClick={() =>
                              isLandlord
                                ? handleLandlordUnhappy(request.id, description)
                                : handleTenantUnhappy(request.id, description)
                            }
                            size="sm"
                          >
                            {isLandlord
                              ? "Отменить бронь арендодатель"
                              : "Отменить бронь арендатор"}
                          </Button>
                        </Tooltip>
                      </VStack>
                    </Box>
                  )}
                </Box>
              ) : (
                // Иначе — кнопка отмены запроса
                <Button
                variant="solid"
                bg="black"
                color="white"
                w="50%"
                size="md"
                _hover={{ bg: "red.600" }}
                onClick={() => handleReject(request.id)}
              >
                Отклонить запрос
              </Button>
              )}
              
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
          <Box mt={4}>
            <HStack spacing={4}>
              {/* Кнопка перехода к владельцу */}
              <Button
                variant="solid"
                bg="black"
                color="white"
                w="50%"
                size="md"
                _hover={{ bg: "blue.600" }}
                onClick={() => {
                  try {
                    navigate(`/user/${request.idAuthorAdvertisement}`);
                  } catch (error) {
                    console.error("Ошибка при переходе к владельцу:", error);
                  }
                }}
              >
                Владелец
              </Button>

              {/* Если бронь началась — показать кнопку "Пожаловаться" и форму */}
              {request.approvalStatus === "Бронь началась" ? (
                <Box w="50%">
                  <Button
                    onClick={handleShowForm}
                    colorScheme="gray"
                    size="md"
                    w="100%"
                  >
                    {showComplaintForm ? "Скрыть" : "Отменить бронь"}
                  </Button>

                  {showComplaintForm && (
                    <Box mt={4}>
                      <VStack align="start" spacing={3}>
                        <Textarea
                          value={description}
                          onChange={(e) => setDescription(e.target.value)}
                          placeholder="Опишите проблему"
                          size="sm"
                        />
                        <Tooltip
                          content={
                            isLandlord
                              ? "Сообщить о недовольстве арендодателя"
                              : "Если вы заехали и хотите отменить бронь"
                          }
                          hasArrow
                          placement="top"
                        >
                          <Button
                            colorScheme="red"
                            isLoading={loading}
                            onClick={() =>
                              isLandlord
                                ? handleLandlordUnhappy(request.id, description)
                                : handleTenantUnhappy(request.id, description)
                            }
                            size="sm"
                          >
                            {isLandlord
                              ? "Я уверен"
                              : "Я уверен"}
                          </Button>
                        </Tooltip>
                      </VStack>
                    </Box>
                  )}
                </Box>
              ) : (
                // Иначе — кнопка отмены запроса
                <Button
                  variant="solid"
                  bg="black"
                  color="white"
                  w="50%"
                  size="md"
                  _hover={{ bg: "red.600" }}
                  onClick={() => handleDelete(request.id)}
                >
                  Отменить запрос
                </Button>
              )}
            </HStack>
          </Box>
        )}
 {id?.toString() === request.idAuthorRentalRequest?.toString() && (
<>  <Flex justify="center" mt={4}>
          {request.approvalStatus === "Оплачен" && (
            <Button
              colorScheme="teal"
              onClick={() => handleStart(request.id)}
              isLoading={loading}
              loadingText="Обработка..."
            >
              Подтвердить заезд
            </Button>
          )}
        </Flex>

        {(request.approvalStatus === "Одобрен" || request.approvalStatus === "Внесена предоплата" || request.approvalStatus === "Внесен залог") && (
          <HStack spacing={4} mt={2}>
            <Button
              variant="solid"
              bg="black"
              color="white"
              w="50%"
              size="md"
              _hover={{ bg: "green" }}
              onClick={async () => {
                try {
                  await handlePaymentRequest(request.paymentId);
                } catch (error) {
                  console.error("Ошибка при обработке платежа:", error);
                }
              }}
            >
              Внести предоплату
            </Button>
            <Button
              variant="solid"
              bg="black"
              color="white"
              w="50%"
              size="md"
              _hover={{ bg: "green" }}
              onClick={async () => {
                try {
                  await handlePaymentRequest(request.paymentActiveDepositId);
                } catch (error) {
                  console.error("Ошибка при обработке платежа:", error);
                }
              }}
            >
              Внести залог
            </Button>
          </HStack>
        )}</>

 )}
      
      </Stack>
    </Box>
  )}
</Card.Footer>
    </Card.Root>
  );
}