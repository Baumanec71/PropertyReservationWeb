import { Button, Box, Stack, Text, Heading, Fieldset, Input } from "@chakra-ui/react";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { changePassword } from "../../services/changePassword";
import { Field } from "@/components/ui/field";
import { getUserEmail } from "../../services/InfoJwt/getUserEmail";

export default function ChangePassword() {
  const [oldPassword, setOldPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [newPasswordConfirm, setNewPasswordConfirm] = useState("");
  const [errorMessages, setErrorMessages] = useState({});
  const [okMessage, setOkMessage] = useState(""); // Строка для успешного сообщения
  const [isLoading, setIsLoading] = useState(false);
  const [email, setEmail] = useState(null);
  const navigate = useNavigate();

  const handleSubmit = async () => {
    setErrorMessages({});
    setOkMessage("");
    setIsLoading(true);
    setIsLoading(false);
    const response = await changePassword(oldPassword, newPassword, newPasswordConfirm); //getUserEmail()

    if (!response.success) {
      setErrorMessages(response.errors);
    } else {
      setOkMessage("Пароль обновлен!");
      setOldPassword("");
      setNewPassword("");
      setNewPasswordConfirm("");
    }
  };

  return (
    <Box
      position="relative"
      maxW="lg"
      p={8}
      borderWidth={1}
      bg = "white"
      color = "black"
      borderRadius="md"
      mx="auto"
      mt={10}
      boxShadow="lg"
    >
      <Heading as="h2" size="lg" mb={4} textAlign="center">
        Смена пароля
      </Heading>
      <Text mb={4} textAlign="center">
        Введите текущий пароль и новый пароль
      </Text>

      <Fieldset.Root size="lg" maxW="md" color = "black">
        <Fieldset.Legend color = "black">Изменение пароля</Fieldset.Legend>
        <Fieldset.HelperText color = "black">
          Пожалуйста, заполните форму для смены пароля.
        </Fieldset.HelperText>

        <Fieldset.Content>
          <Field label="Текущий пароль">
            <Input
              name="oldPassword"
              type="password"
              value={oldPassword}
              onChange={(e) => setOldPassword(e.target.value)}
            />
          </Field>

          <Field label="Новый пароль">
            <Input
              name="newPassword"
              type="password"
              value={newPassword}
              onChange={(e) => setNewPassword(e.target.value)}
            />
          </Field>

          <Field label="Подтверждение нового пароля">
            <Input
              name="newPasswordConfirm"
              type="password"
              value={newPasswordConfirm}
              onChange={(e) => setNewPasswordConfirm(e.target.value)}
            />
          </Field>
        </Fieldset.Content>

        {/* Вывод сообщения об успехе */}
        {okMessage && (
          <Box mt={4} p={2} bg="green.100" borderRadius="md">
            <Text color="green.700">{okMessage}</Text>
          </Box>
        )}

        {/* Вывод сообщений об ошибках */}
        {Object.keys(errorMessages).map((field, index) => (
          <Box key={index}>
            {(Array.isArray(errorMessages[field])
              ? errorMessages[field]
              : [errorMessages[field]]
            ).map((error, idx) => (
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
          loadingText="Обновление..."
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
            bg: "#FDD835",
            transform: "scale(1.04)",
            color: "black",
            boxShadow: "0 6px 14px rgba(253, 216, 53, 0.35)"
          }}
          _active={{
            transform: "scale(0.98)",
            boxShadow: "0 2px 6px rgba(253, 216, 53, 0.2)"
          }}
        >
          Обновить пароль
        </Button>

        <Button
        variant="link"
   w="100%"
   px={4}
   py={2}
   rounded="lg"
   color="red"
   fontWeight="medium"
   _hover={{ transform: "scale(1.03)" }}
          onClick={() => navigate("/me")}
        >
          Назад
        </Button>
      </Fieldset.Root>
    </Box>
  );
}