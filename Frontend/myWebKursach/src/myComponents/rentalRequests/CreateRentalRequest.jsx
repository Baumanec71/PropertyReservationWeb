import React, { useState, useEffect } from "react";
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";
import axios from "axios";
import { Box, Button, Spinner, Text, Heading, useBreakpointValue, Fieldset, Field, Stack, Input  } from "@chakra-ui/react";
import { Toaster, toaster } from "@/components/ui/toaster";
import { useNavigate, useParams } from "react-router-dom";
import { getRentalRequestForm, createRentalRequest } from "../../services/rentalRequests/createRentalRequest";
import { getUserEmail } from "../../services/InfoJwt/getUserEmail";
import { registerLocale, setDefaultLocale } from "react-datepicker";
import ru from "date-fns/locale/ru"; // Импортируем русскую локализацию
registerLocale("ru", ru);
setDefaultLocale("ru");
import { parse } from 'date-fns';
import { differenceInDays, addDays } from "date-fns";



export default function CreateRentalRequest() {
      const scale = useBreakpointValue({
        base: "0em",
        sm: "30em",
        md: "48em",
        lg: "62em",
        xl: "80em",
        "2xl": "96em",
      });
    const { idNeedAdvertisement } = useParams();
    const [bookedDates, setBookedDates] = useState([]);
    const [loading, setLoading] = useState(true);
    const [formData, setFormData] = useState({
        bookingStartDate: new Date(),  // Устанавливаем текущую дату по умолчанию
        bookedDates: bookedDates,
        bookingFinishDate: addDays(new Date(), 1),
        idNeedAdvertisement: idNeedAdvertisement,
        price: 0,
        checkInTime: "",   // Время заезда
        checkOutTime: "",  // Время выезда
    });
    const [errorMessages, setErrorMessages] = useState({});
    const [okMessage, setOkMessage] = useState(""); // Строка для успешного сообщения
    const [error, setError] = useState("");
    const [email, setEmail] = useState(null);
    const navigate = useNavigate();


    // Загрузка забронированных дат
    useEffect(() => {
        if (!idNeedAdvertisement) {
            setError("ID объявления не передано.");
            return;
        }
        async function fetchFormData() {
            setLoading(true);
            try {
                const result = await getRentalRequestForm(idNeedAdvertisement);
                if (result.success) {
                    setFormData((prevData) => ({
                        ...prevData,
                        bookingStartDate: result.data.bookingStartDate || new Date(),
                        bookingFinishDate: result.data.bookingFinishDate || addDays(new Date(), 1),
                        price: result.data.rentalPrice || 0,
                        checkInTime: result.data.checkInTime || 0,
                        checkOutTime: result.data.checkOutTime || 0,
                    
                    }));
                    setBookedDates(result.data.bookedDates || []);
                    const token = localStorage.getItem("authToken");
                    if (token) {
                        const userEmail = await getUserEmail();
                        setEmail(userEmail);
                        setFormData((prevData) => ({
                            ...prevData,
                            login: userEmail,
                        }));
                    } else {
                        setEmail(null);
                    }
                } else {
                    setError(result.error);
                }
            } catch (error) {
                console.error("Ошибка при загрузке данных формы:", error);
                setError("Не удалось загрузить данные.");
            } finally {
                setLoading(false);
            }
        }
        fetchFormData();
    }, [idNeedAdvertisement]);

    // Отправка заявки
    const handleSubmit = async () => {
        setErrorMessages({});
        setOkMessage("");
        let error = false;
        if (!formData.bookingStartDate || isNaN(formData.bookingStartDate)) {
            setErrorMessages((prev) => ({
                ...prev,
                BookingStartDate: "Укажите начало бронирования",
            }));
            error = true;
        }

        if (!formData.bookingFinishDate || isNaN(formData.bookingFinishDate)) {
            setErrorMessages((prev) => ({
                ...prev,
                BookingFinishDate: "Укажите конец бронирования",
            }));
            error = true;
        }

        if (error) {
            return;
        }

        const result = await createRentalRequest(formData);
        if (result.success) {
            alert(result.data)
            setOkMessage("Запрос на аренду отправлен!");
        } else {
            setErrorMessages(result.errors || { general: "Ошибка при создании запроса на бронирование" });
        }
    };

      const getDaysBetween = (start, end) => {
        return differenceInDays(end, start) + 1;
    };
     const daysBetween = getDaysBetween(formData.bookingStartDate, formData.bookingFinishDate);
     const totalPrice = daysBetween * formData.price;
     const excludeDateIntervals = [];
     for (let i = 0; i < bookedDates.length; i += 2) {
         const startDate = new Date(bookedDates[i]);
         const endDate = new Date(bookedDates[i + 1]);
     
         // Устанавливаем время на начало дня (00:00:00) для startDate
         startDate.setHours(0, 0, 0, 0);
     
         // Устанавливаем время на конец дня (23:59:59.999) для endDate
         endDate.setHours(23, 59, 59, 999);
     
         excludeDateIntervals.push({ start: startDate, end: endDate });
     }
     

    return (
        <Box
            transform={`scale(${scale})`}
            transition="transform 0.2s ease-in-out"
            position="relative"
            maxW="lg"
            p={8}
            borderWidth={1}
            borderRadius="md"
            mx="auto"
            mt={10}
            boxShadow="lg"
            bg="white"
            borderColor="gray.300"
        >
            <Heading as="h2" size="lg" mb={4} color="gray.800">
                Создание заявки на аренду
            </Heading>

            {loading ? (
                <Box display="flex" justifyContent="center" alignItems="center" height="200px">
                    <Spinner size="lg" />
                </Box>
            ) : (
                <Fieldset.Root size="lg" maxW="md">
                    <Stack spacing={4}>
                        <Fieldset.Legend fontSize="lg" fontWeight="bold">
                            Цена за день {formData.price} ₽
                        </Fieldset.Legend>
                        <Fieldset.HelperText>
                            Выбрано {daysBetween} дней общая стоимость {totalPrice}
                        </Fieldset.HelperText>
                    </Stack>

                    <Fieldset.Content>
                        <Field.Root>
                            <Field.Label>Диапазон дат</Field.Label>
        
                            <DatePicker
                                selected={formData.bookingStartDate}
                                onChange={(date) => {
                                    setFormData({
                                        ...formData,
                                        bookingStartDate: date[0], // Начальная дата
                                        bookingFinishDate: date[1], // Конечная дата
                                    });
                                }}
                                selectsRange
                                inline
                                startDate={formData.bookingStartDate}
                                endDate={formData.bookingFinishDate}
                                excludeDateIntervals={excludeDateIntervals}
                                minDate={new Date()}
                                dateFormat="dd/MM/yyyy"
                                className="border p-2 rounded w-full mb-4"
                            />

                        </Field.Root>

                        <Field.Root>
  <Field.Label>Время заезда</Field.Label>
  <Input
    type="time"
    value={formData.checkInTime}
    
    onChange={(e) =>
      setFormData((prev) => ({ ...prev, checkInTime: e.target.value }))
    }
  />
  {errorMessages.CheckInTime && (
    <Text color="red.500" fontSize="sm">{errorMessages.CheckInTime}</Text>
  )}
</Field.Root>

<Field.Root>
  <Field.Label>Время выезда</Field.Label>
  <Input
    type="time"
    value={formData.checkOutTime}
    onChange={(e) =>
      setFormData((prev) => ({ ...prev, checkOutTime: e.target.value }))
    }
  />
  {errorMessages.CheckOutTime && (
    <Text color="red.500" fontSize="sm">{errorMessages.CheckOutTime}</Text>
  )}
</Field.Root>
                    </Fieldset.Content>
                </Fieldset.Root>
                
            )}
                                    {okMessage && (
                                        <Box mt={4} p={2} bg="green.100" borderRadius="md">
                                            <Text color="green.700">{okMessage}</Text>
                                        </Box>
                                    )}
                    
                                    {Object.keys(errorMessages).map((field, index) => (
                                        <Box key={index}>
                                          {console.log(errorMessages)}
                                            {(Array.isArray(errorMessages[field]) ? errorMessages[field] : [errorMessages[field]]).map((error, idx) => (
                                                <Text key={idx} color="red.500" fontSize="sm">
                                                    {error}
                                                </Text>
                                            ))}
                                        </Box>
                                    ))}

            {error && (
                <Box mt={4} p={2} bg="red.100" borderRadius="md">
                    <Text color="red.700">{error}</Text>
                </Box>
            )}

 

            <Button
                variant="solid"
                onClick={handleSubmit}
                width="full"
                mt={4}
                colorScheme="blue"
                isLoading={loading}
                loadingText="Отправка..."
            >
                Отправить запрос на аренду
            </Button>
        </Box>
    );
}