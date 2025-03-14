import { Card, Stack, Text, Avatar, Button } from "@chakra-ui/react";

export default function UserCard({ user, onEdit }) {

  return (
    <Card.Root  >
      <Card.Header>
        <Card.Title>{user.name}</Card.Title>
      </Card.Header>
      <Card.Body>
        <Stack gap="4" w="full" align="center">
        <Avatar.Root boxSize="100px"  mb={4} pos={"relative"}  >
          <Avatar.Image src={user.avatar || "https://via.placeholder.com/150"} />
          <Avatar.Fallback name={user.name} />
        </Avatar.Root>
          <Text><b>ID:</b> {user.id}</Text>
          <Text><b>Почта:</b> {user.email}</Text>
          <Text><b>Роль:</b> {user.role}</Text>
          <Text><b>Номер телефона:</b> {user.phoneNumber}</Text>
          <Text><b>Рейтинг:</b> {user.rating}</Text>
          <Text><b>Созданных объявлений:</b> {user.numberOfAdsCreated}</Text>
          <Text><b>Количество сделок:</b> {user.numberOfTransactions}</Text>
          <Text><b>Статус:</b> {user.deleteStatus ? "Активный" : "Заблокирован"}</Text>
          <Text><b>Дата регистрации:</b> {user.dateOfRegistration}</Text>
        </Stack>
      </Card.Body>
      <Card.Footer justifyContent="flex-end" >
        <Button variant="outline" colorScheme="green" _hover={{ bg: "green.100" } } onClick={onEdit}>Редактировать</Button>
        <Button variant="solid" colorScheme="red" _hover={{ bg: "red.600" }}>Удалить</Button>
      </Card.Footer>
    </Card.Root>
  );
}
