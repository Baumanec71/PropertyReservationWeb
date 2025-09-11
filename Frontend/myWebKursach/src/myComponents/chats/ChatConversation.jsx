
import { Box, Button, Spinner, Text, Heading, useBreakpointValue, Fieldset, Field, Stack, Input, Avatar, VStack, HStack  } from "@chakra-ui/react";
import { useState } from "react";

const mockMessages = [
  {
    id: 1,
    sender: "tenant",
    name: "bob",
    avatar: "https://bit.ly/dan-abramov",
    text: "Добрый день! Подскажите, пожалуйста, есть ли поблизости бесплатная парковка?",
    time: "11:25",
  },
  {
    id: 2,
    sender: "landlord",
    name: "Boris",
    avatar: "https://bit.ly/kent-c-dodds",
    text: "Здравствуйте! Да, прямо через дорогу есть большая общественная парковка — обычно там всегда есть свободные места.",
    time: "11:27",
  },
  {
    id: 3,
    sender: "tenant",
    name: "bob",
    avatar: "https://bit.ly/dan-abramov",
    text: "Отлично, большое спасибо за информацию!",
    time: "11:28",
  },
  {
    id: 4,
    sender: "landlord",
    name: "Boris",
    avatar: "https://bit.ly/kent-c-dodds",
    text: "Не за что! Если будут ещё вопросы — обращайтесь в любое время :)",
    time: "11:29",
  },
];

export default function ChatConversation() {
  const [messages, setMessages] = useState(mockMessages);
  const [newMessage, setNewMessage] = useState("");

  const sendMessage = () => {
    if (!newMessage.trim()) return;

    const newMsg = {
      id: messages.length + 1,
      sender: "landlord", // для примера всегда от арендатора
      name: "Boris",
      avatar: "https://bit.ly/kent-c-dodds",
      text: newMessage,
      time: new Date().toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" }),
    };

    setMessages([...messages, newMsg]);
    setNewMessage("");
  };

  return (
    <Box
      w="full"
      maxW="700px"
      mx="auto"
      mt={6}
      p={4}
      bg="gray.50"
      borderRadius="xl"
      boxShadow="md"
      h="600px"
      display="flex"
      flexDirection="column"
      justifyContent="space-between"
    >
      {/* Сообщения */}
      <VStack spacing={4} overflowY="auto" maxH="500px" pr={2}>
      {  console.log(messages)}
      {messages.map((msg) => ( 
          <HStack
            key={msg.id}
            w="full"
            justify={msg.sender === "tenant" ? "flex-start" : "flex-end"}
          >
                  <Avatar.Root boxSize="50px" mb={2} border="2px solid #FDD835" borderRadius="full">
        <Avatar.Image src={msg.avatar || "https://via.placeholder.com/150"} />
      </Avatar.Root>
            <Box
              bg={msg.sender === "tenant" ? "gray.200" : "#FDD835"}
              color={msg.sender === "tenant" ? "black" : "black"}
              px={4}
              py={2}
              borderRadius="xl"
              maxW="70%"
              boxShadow="sm"
            >
              <Text fontSize="sm" fontWeight="semibold">
                {msg.name}
              </Text>
              <Text>{msg.text}</Text>
              <Text fontSize="xs" mt={1} textAlign="right" opacity={0.6}>
                {msg.time}
              </Text>
            </Box>
          </HStack>
        ))}
      </VStack>

      {/* Ввод нового сообщения */}
      <HStack mt={4} pt={2} borderTop="1px solid" borderColor="gray.200">
        <Input
          placeholder="Введите сообщение..."
          value={newMessage}
          onChange={(e) => setNewMessage(e.target.value)}
          onKeyDown={(e) => e.key === "Enter" && sendMessage()}
          bg="white"
        />
        <Button                         size="lg"
                        bg="#111111"
                        color="white"
                        fontWeight="semibold"
                        rounded="xl"
                        transition="all 0.3s ease"
                        _hover={{
                          bg: "#FDD835",
                          transform: "scale(1.04)",
                          color: "black",
                          boxShadow: "0 6px 14px rgba(253, 216, 53, 0.35)",
                        }}
                        _active={{
                          transform: "scale(0.98)",
                          boxShadow: "0 2px 6px rgba(253, 216, 53, 0.2)",
                        }}
        w="20%" onClick={sendMessage}>
          Отправить
        </Button>
      </HStack>
    </Box>
  );
}