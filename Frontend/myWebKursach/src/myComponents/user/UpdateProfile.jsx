import axios from "axios";
import { useState, useEffect } from "react";
import { Box, Button, Input, Text, Image, Avatar } from "@chakra-ui/react";
import { Toaster, toaster } from "@/components/ui/toaster";
import { FaTimes } from "react-icons/fa";
import InputMask from 'react-input-mask';

export default function UpdateProfile({ user, onUpdate, onCancel }) {
  const [name, setName] = useState("");
  const [avatar, setAvatar] = useState(null);
  const [preview, setPreview] = useState(null);
  const [phoneNumber, setPhoneNumber] = useState("");

  useEffect(() => {
    if (user) {
      setName(user.name || "");
      setPhoneNumber(user.phoneNumber || "");
      setPreview(user.avatar || null);
    }
  }, [user]);

  const handleUpdate = async () => {
    const token = localStorage.getItem("authToken");
    if (!token) {
      toaster.create({
        title: "Ошибка",
        description: "Токен отсутствует, авторизуйтесь снова.",
        status: "error",
        duration: 3000,
        isClosable: true,
      });
      return;
    }

    let avatarBase64 = preview ? preview.split(",")[1] : null;
    if (avatar) {
      avatarBase64 = await convertFileToBase64(avatar);
    }

    const requestData = {
      name: name || null,
      phoneNumber: phoneNumber || null,
      avatar: avatarBase64,
    };

    try {
      const response = await axios.put(
        `${API_BASE_URL}/api/User/Update`,
        requestData,
        {
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
        }
      );

      toaster.create({
        title: "Успех",
        description: response.data.description || "Данные успешно обновлены.",
        status: "success",
        duration: 3000,
        isClosable: true,
      });

      onUpdate();
    } catch (error) {
      toaster.create({
        title: "Ошибка",
        description: error.response?.data?.error || "Неизвестная ошибка",
        status: "error",
        duration: 3000,
        isClosable: true,
      });
    }
  };

  const handleFileChange = (e) => {
    const file = e.target.files[0];
    setAvatar(file);
    if (file) {
      const reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = () => setPreview(reader.result);
    }
  };

  const convertFileToBase64 = (file) => {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = () => resolve(reader.result.split(",")[1]);
      reader.onerror = (error) => reject(error);
    });
  };

  return (
    <Box
    display="flex"
    flexDirection="column"
    alignItems="center"
    p={8}
    w="100%"
    maxW="450px"
    borderWidth="1px"
    borderRadius="2xl"
    boxShadow="xl"
    bg="white"
    mx="auto"
  >
    <Text fontSize="2xl" fontWeight="bold" color="teal.600" mb={6}>
      Редактирование профиля
    </Text>
  
    <Box position="relative" mb={4}>
      {preview ? (
        <>
          <Image
            src={preview}
            alt="User Icon"
            boxSize="120px"
            borderRadius="full"
            objectFit="cover"
            border="2px solid #E2E8F0"
          />
          <Button
            position="absolute"
            top="-6px"
            right="-6px"
            size="xs"
            colorScheme="red"
            borderRadius="full"
            onClick={() => {
              setAvatar(null);
              setPreview(null);
            }}
          >
            <FaTimes size={12} />
          </Button>
        </>
      ) : (
        <Avatar.Root boxSize="120px" mb={2} border="2px solid #FDD835" borderRadius="full">
        <Avatar.Image src={user.avatar || "https://via.placeholder.com/150"} />
        <Avatar.Fallback name={user.name} />
      </Avatar.Root>
        //<Avatar boxSize="120px" />
      )}
      <Button
        mt={3}
        size="sm"
        variant="outline"
        colorScheme="teal"
        onClick={() => document.getElementById("fileInput").click()}
      >
        Сменить аватар
      </Button>
      <Input type="file" id="fileInput" hidden onChange={handleFileChange} />
    </Box>
  
    <Box w="full" mb={4}>
      <Text fontSize="sm" fontWeight="semibold" color="gray.700" mb={1}>
        Имя пользователя *
      </Text>
      <Input
        type="text"
        value={name}
        onChange={(e) => setName(e.target.value)}
        minLength={2}
        maxLength={20}
        placeholder="Введите имя"
        focusBorderColor="teal.500"
      />
    </Box>
  
    <Box w="full" mb={4}>
  <Text fontSize="sm" fontWeight="semibold" color="gray.700" mb={1}>
    Номер телефона *
  </Text>
  <InputMask
    mask="+7 (999) 999-99-99"
    value={phoneNumber}
    onChange={(e) => setPhoneNumber(e.target.value)}
  >
    {(inputProps) => (
      <Input
        {...inputProps}
        type="tel"
        placeholder="+7 (___) ___-__-__"
        focusBorderColor="teal.500"
      />
    )}
  </InputMask>
</Box>
  
    <Box display="flex" gap={3} mt={4} w="full">
      <Button
        colorScheme="gray"
        variant="outline"
        flex={1}
        onClick={onCancel}
      >
        Назад
      </Button>
      <Button
        colorScheme="teal"
        flex={1}
        onClick={handleUpdate}
      >
        Сохранить
      </Button>
    </Box>
  
    <Toaster />
  </Box>
  );
}
