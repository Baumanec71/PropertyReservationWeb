import { Button, Box, Text, Heading, Fieldset, Input, useBreakpointValue, HStack } from "@chakra-ui/react";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { login } from "../services/login";
import { Field } from "@/components/ui/field";
import { getUserEmail } from "../services/InfoJwt/getUserEmail";

export default function Login() {
      const scale = useBreakpointValue({     base: "0em", // 0px
        sm: "30em", // ~480px
        md: "48em", // ~768px
        lg: "62em", // ~992px
        xl: "80em", // ~1280px
        "2xl": "96em", }); //  ~1536px Масштабирование в зависимости от экрана
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [errorMessages, setErrorMessages] = useState([]);
    const [okMessage, setOkMessage] = useState("");
    const [isLoading, setIsLoading] = useState(false);
    const navigate = useNavigate();

    const handleSubmit = async () => {
        setErrorMessages([]);
        setOkMessage(""); // Clear before submission
        setIsLoading(true);

        const response = await login(email, password);

        setIsLoading(false);

        if (!response.success) {
          console.log(response)
            setErrorMessages(response.errors);
        } else {
            setOkMessage("Вы успешно вошли!");
            localStorage.setItem("authToken", response.data.token);
            navigate("/Advertisements/1"); // Redirect to homepage or profile
        }
    };

    const color = "black"; // Цвет текста - черный
    const bg = "#FAFAFA"; // Основной фон - светлый
    const accentColor = "#FF4081"; // Акцентный цвет для кнопок
    return (
        <Box transform={`scale(${scale})`} transition="transform 0.2s ease-in-out"  backgroundImage="url('/public/bgsity.jpg')"
            position="relative"
            maxW="lg"
            bg="#FFFFFF"
            color={color}
            boxShadow="0 4px 12px rgba(0, 0, 0, 0.1)"  //"0 4px 12px rgba(0, 0, 0, 0.1)"
            borderRadius="md"
            border="1px solid #E0E0E0"
            p={8}
            borderWidth={3}
            borderColor="white"
            mx="auto"
            mt={10}
          //  boxShadow="lg"
        >
            <Heading as="h2" size="lg" mb={4} textAlign="center">
                Вход в систему
            </Heading>
            <Text mb={4} color= {color} textAlign="center">Введите ваши учетные данные</Text>

            <Fieldset.Root size="lg" maxW="md" color = {color}>
                <Fieldset.Legend color = {color}>Авторизация</Fieldset.Legend>
                <Fieldset.Content>
                    <Field label="Почта">
                        <Input
                            name="email"
                            type="email"
                            borderColor={color}
                            borderWidth={1}
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                        />
                    </Field>

                    <Field label="Пароль" >
                        <Input
                            name="password"
                            type="password"
                            borderColor={color}
                            borderWidth={1}
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                        />
                    </Field>
                </Fieldset.Content>

                {/* Show error or success messages */}
                {okMessage && (
                    <Box mt={4} p={2} bg="green.100" borderRadius="md">
                        <Text color="green.700">{okMessage}</Text>
                    </Box>
                )}

                {errorMessages.length > 0 && (
                    <Box mt={4}>
                        {errorMessages.map((error, index) => (
                            <Text key={index} color="red.500" fontSize="sm">
                                {error}
                            </Text>
                        ))}
                    </Box>
                )}

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
  Войти
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
  onClick={() => navigate("/create-account")}
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
  Создать аккаунт
</Button>
                  </HStack>
            </Fieldset.Root>
        </Box>
    );
}