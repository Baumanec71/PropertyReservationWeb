import axios from "axios";
import { useState, useEffect } from "react";
import { Box, Button, Input, Text, Image } from "@chakra-ui/react";
import { Toaster, toaster } from "@/components/ui/toaster";
import { FaTimes } from "react-icons/fa";

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
        "https://localhost:7069/api/User/Update",
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
      p={6}
      w="100%"
      maxW="400px"
      borderWidth="1px"
      borderRadius="lg"
      boxShadow="md"
      bg="white"
    >
      <Text fontSize="xl" fontWeight="bold" mb={4}>
        Редактирование профиля
      </Text>
      <Box position="relative" mb={4}>
        {preview && (
          <>
            <Image
              src={preview}
              alt="User Icon"
              boxSize="100px"
              borderRadius="full"
              objectFit="cover"
            />
            <Button
              position="absolute"
              top="-5px"
              right="-5px"
              size="xs"
              colorScheme="red"
              borderRadius="full"
              onClick={() => {
                setAvatar(null);
                setPreview(null);
              }}
            >
              <FaTimes size={14} />
            </Button>
          </>
        )}
        <Button mt={2} onClick={() => document.getElementById("fileInput").click()}>
          Смена аватара
        </Button>
        <Input
          type="file"
          id="fileInput"
          hidden
          onChange={handleFileChange}
        />
      </Box>
      <Box w="full" mb={3}>
        <Text fontSize="md" fontWeight="bold" mb={1}>
          User name *
        </Text>
        <Input
          type="text"
          value={name}
          onChange={(e) => setName(e.target.value)}
          minLength={2}
          maxLength={20}
        />
      </Box>
      <Box w="full" mb={3}>
        <Text fontSize="md" fontWeight="bold" mb={1}>
          Номер телефона *
        </Text>
        <Input
          type="email"
          value={phoneNumber}
          onChange={(e) => setPhoneNumber(e.target.value)}
        />
      </Box>
      <Box display="flex" gap={2} mt={4} w="full">
        <Button colorScheme="red" flex={1} onClick={onCancel}>
          Назад
        </Button>
        <Button colorScheme="blue" flex={1} onClick={handleUpdate}>
          Применить
        </Button>
      </Box>
      <Toaster />
    </Box>
  );
}
