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

   // const token = localStorage.getItem("authToken");
  //  if (token) {
   //   setEmail(getUserEmail());
  //  } else {
  //    setEmail(null);
  //  }
    setIsLoading(false);
    const response = await changePassword(oldPassword, newPassword, newPasswordConfirm); //getUserEmail()

    if (!response.success) {
      setErrorMessages(response.errors);
    } else {
      setOkMessage("Пароль обновлен!");
      // Очищаем поля ввода
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

      <Fieldset.Root size="lg" maxW="md">
        <Fieldset.Legend>Изменение пароля</Fieldset.Legend>
        <Fieldset.HelperText>
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
          mt={4}
        >
          Обновить пароль
        </Button>

        <Button
          variant="outline"
          mt={4}
          width="full"
          onClick={() => navigate("/")}
        >
          Назад
        </Button>
      </Fieldset.Root>
    </Box>
  );
}