import { Button, Box, useBreakpointValue, Text, Heading, Fieldset, Input, HStack } from "@chakra-ui/react";
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
            navigate("/Advertisements/1"); // Перенаправляем на главную страницу или на страницу профиля
        }
    };
    const isValidEmail = (email) =>
        /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/.test(email);
    const color = "black";
    const bg = `${BG}`;
    return (
        <Box transform={`scale(${scale})`} transition="transform 0.2s ease-in-out"
            position="relative"
            maxW="lg"
            color = {color}
            bg = "#FFFFFF"
            boxShadow="0 4px 12px rgba(0, 0, 0, 0.1)"  //"0 4px 12px rgba(0, 0, 0, 0.1)"
            borderRadius="md"
            border="1px solid #E0E0E0"
            p={8}
            borderWidth={3}
            mx="auto"
            mt={10}
            borderColor="white"
        >
            <Heading as="h2" size="lg" mb={4} textAlign="center" color = {color}>
                Регистрация
            </Heading>
            <Fieldset.Root size="lg" maxW="md">

                <Fieldset.Content>
                    <Field label="Почта" >
                        <Input
                           borderWidth={1}
                           borderColor={color}
                            name="email"
                            type="email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            focusBorderColor="teal.500"
                            isInvalid={email && !/^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/.test(email)}
                          />
                           {email && !/^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/.test(email) && (
    <Text fontSize="xs" color="red.500" mt={1}>
      Неверный формат email
    </Text>
  )}
                    </Field>

                    <Field label="Пароль">
                        <Input
                            borderWidth={1}
                            borderColor={color}
                            name="password"
                            type="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                        />
                    </Field>

                    <Field label="Подтверждение пароля">
                        <Input
                            borderWidth={1}
                            borderColor={color}
                            name="passwordConfirm"
                            type="password"
                            value={passwordConfirm}
                            onChange={(e) => setPasswordConfirm(e.target.value)}
                        />
                    </Field>
                </Fieldset.Content>
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
  onClick={handleSubmit}
    isLoading={isLoading}
  loadingText="Вход..."
  width="full"
  size="lg"
  px={6}
  py={3}
  mt={4}
  bg="#111111"
  //bg="#FFEB3B"
  color="white"
  fontWeight="semibold"
  rounded="xl"
 
  transition="all 0.3s ease"
  _hover={{
    bg: "#FDD835", // чуть темнее при наведении
    transform: "scale(1.04)",
    color: "black",
    boxShadow: "0 6px 14px rgba(253, 216, 53, 0.35)"
  }}
  _active={{
    transform: "scale(0.98)",
    boxShadow: "0 2px 6px rgba(253, 216, 53, 0.2)"
  }}
>
  Зарегистрироваться
</Button>

<HStack mt={4} spacing={4}>
<Button
                  variant="link"
                    onClick={() => navigate("/Advertisements/1")}
                    w="50%"
                    px={4}
                    py={2}
                    rounded="lg"
                    color="red"
                    fontWeight="medium"
                    _hover={{ transform: "scale(1.03)" }}
                  >
                    Назад
                  </Button>

  <Button
    onClick={() => navigate("/login")}
    w="50%"
    px={4}
    py={2}
    rounded="lg"
    variant="outline"
    borderColor="#E0E0E0"
    color="#111111"
    // boxShadow="0 4px 10px rgba(255, 235, 59, 0.3)"
    fontWeight="medium"
    _hover={{
      bg: "#FFF9C4",
      transform: "scale(1.03)"
    }}
    transition="all 0.2s"
  >
    Войти в аккаунт
  </Button>
</HStack>
               
            </Fieldset.Root>
        </Box>
    );
}