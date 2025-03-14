import { Button, Box, useBreakpointValue, Text, Heading, Fieldset, Input } from "@chakra-ui/react";
import { useState } from "react";
import { useNavigate } from "react-router-dom"; // Для навигации
import { registration } from "../services/registration";
import { Field } from "@/components/ui/field";

export default function CreateAccount() {
    const scale = useBreakpointValue({     base: "0em", // 0px
        sm: "30em", // ~480px
        md: "48em", // ~768px
        lg: "62em", // ~992px
        xl: "80em", // ~1280px
        "2xl": "96em", }); //  ~1536px Масштабирование в зависимости от экрана
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [passwordConfirm, setPasswordConfirm] = useState("");
    const [errorMessages, setErrorMessages] = useState({});
    const [okMessage, setOkMessage] = useState(""); // Строка для успешного сообщения
    const [isLoading, setIsLoading] = useState(false);
    const navigate = useNavigate(); // Для перехода на страницу входа

    const handleSubmit = async () => {
        setErrorMessages({});
        setOkMessage(""); // Очищаем сообщение перед отправкой
        setIsLoading(true);

        const response = await registration(email, password, passwordConfirm);

        setIsLoading(false);

        if (!response.success) {
            setErrorMessages(response.errors);
        } else {
            setOkMessage("Вы успешно зарегистрировались!"); // Выводим сообщение
            localStorage.setItem("authToken", response.data.token); // Сохраняем токен в локальное хранилище
            navigate("/"); // Перенаправляем на главную страницу или на страницу профиля
        }
    };

    return (
        <Box transform={`scale(${scale})`} transition="transform 0.2s ease-in-out"
            position="relative"
            maxW="lg"
            p={8}
            borderWidth={1}
            borderRadius="md"
            mx="auto"
            mt={10}
            boxShadow="lg"
        >
            <Heading as="h2" size="lg" mb={4} textAlign="center">
                Регистрация
            </Heading>
            <Text mb={4} textAlign="center">Заполните форму для регистрации</Text>

            <Fieldset.Root size="lg" maxW="md">
                <Fieldset.Legend>Регистрация нового аккаунта</Fieldset.Legend>
                <Fieldset.HelperText>
                    Пожалуйста, заполните данные для создания аккаунта.
                </Fieldset.HelperText>

                <Fieldset.Content>
                    <Field label="Почта">
                        <Input
                            name="email"
                            type="email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                        />
                    </Field>

                    <Field label="Пароль">
                        <Input
                            name="password"
                            type="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                        />
                    </Field>

                    <Field label="Подтверждение пароля">
                        <Input
                            name="passwordConfirm"
                            type="password"
                            value={passwordConfirm}
                            onChange={(e) => setPasswordConfirm(e.target.value)}
                        />
                    </Field>
                </Fieldset.Content>

                {/* Вывод сообщений об ошибках и успешной регистрации */}
                {okMessage && (
                    <Box mt={4} p={2} bg="green.100" borderRadius="md">
                        <Text color="green.700">{okMessage}</Text>
                    </Box>
                )}

                {Object.keys(errorMessages).map((field, index) => (
                    <Box key={index}>
                        {(Array.isArray(errorMessages[field]) ? errorMessages[field] : [errorMessages[field]]).map((error, idx) => (
                            <Text key={idx} color="red.500" fontSize="sm">
                                {error}
                            </Text>
                        ))}
                    </Box>
                ))}

                <Button
                    variant="solid"
                    onClick={handleSubmit}
                    isLoading={isLoading}
                    loadingText="Загружается"
                    width="full"
                    mt={4}
                >
                    Зарегистрироваться
                </Button>

                <Button
                    variant="outline"
                    mt={4}
                    width="full"
                    onClick={() => navigate("/login")} // Переход на страницу входа
                >
                    Войти в аккаунт
                </Button>

                <Button
                    variant="link"
                    colorScheme="blue"
                    mt={4}
                    width="full"
                    onClick={() => navigate("/")} // Кнопка "Назад" возвращает на главную
                >
                    Назад
                </Button>
            </Fieldset.Root>
        </Box>
    );
}