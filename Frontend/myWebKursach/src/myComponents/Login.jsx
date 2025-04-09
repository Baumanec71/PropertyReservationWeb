import { Button, Box, Text, Heading, Fieldset, Input, useBreakpointValue } from "@chakra-ui/react";
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
            setErrorMessages(response.errors);
        } else {
            setOkMessage("Вы успешно вошли!");
            localStorage.setItem("authToken", response.data.token);
            navigate("/Advertisements/1"); // Redirect to homepage or profile
        }
    };
    const color = `${COLOR}`;
    const bg = `${BG}`;
    return (
         // appearance="dark" colorPalette="red" forcedTheme="dark"  bg = "red.500"
        <Box transform={`scale(${scale})`} transition="transform 0.2s ease-in-out" color = {color} bg = {bg}
            position="relative"
            maxW="lg"
            p={8}
            borderWidth={3}
           // colorPalette="black"
            borderColor="white"

            borderRadius="md"
            mx="auto"
            mt={10}
            boxShadow="lg"
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
                            borderColor="white"
                            borderWidth={1}
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                        />
                    </Field>

                    <Field label="Пароль" >
                        <Input
                            name="password"
                            type="password"
                            borderColor="white"
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
                    variant="outline"
                    onClick={handleSubmit}
                    isLoading={isLoading}
                    px={6}
                    py={3}
                    color = "white"
                  //  bg = "green"
                  size = "lg"
                    loadingText="Вход..."
                    width="full"
                    mt={4}
                    borderWidth="2px"
  colorPalette="green"
  borderColor="colorPalette.500"
  _hover={{
    bg: "colorPalette.600",
  }}
                >
                    Войти
                </Button>

                <Button 
                    variant="outline"
                    mt={1}
                    px={6}
                    py={3}
                    size = "lg"
                    color = "white"
                    borderWidth="2px"
  colorPalette="blue"
  borderColor="colorPalette.500"
  _hover={{
    bg: "colorPalette.600",
  }}
                    //colorPalette="pink"
                   // bg = "blue"
                    width="full"
                    onClick={() => navigate("/create-account")} // Redirect to create account page
                >
                    Создать аккаунт
                </Button>

                <Button 
                    variant="link"
                    colorScheme="red"
                    mt={1}
                    width="full"
                    borderWidth="2px"
                    colorPalette="white"
                    borderColor="colorPalette.500"
                    _hover={{
                      bg: "colorPalette.600",
                    }}
                    onClick={() => navigate("/Advertisements/1")}
                >
                    Назад
                </Button>
            </Fieldset.Root>
        </Box>
    );
}