import { Card, Text, Button, Image, Container, chakra, Stack, SimpleGrid,
    VisuallyHidden,  List, HStack, VStack, IconButton, Heading, Flex , Grid, GridItem, Tag, Box, Wrap, WrapItem, Collapsible,
    RatingGroup, CloseButton, Dialog, Portal   } from "@chakra-ui/react";
import { useNavigate } from "react-router-dom";
import { FaChevronLeft, FaChevronRight } from 'react-icons/fa6'
import { useState, useEffect } from "react";
import { useBreakpointValue } from "@chakra-ui/react";
import { YMaps, Map, Placemark} from "@pbe/react-yandex-maps";
import { getUserRole } from "../../services/InfoJwt/getUserRole";
import { getUserId } from "../../services/InfoJwt/getUserId";
import {deleteAdvertisementForUser} from "../../services/advertisements/deleteAdvertisementForUser";
import {approvedAdvertisementFalseForAdmin} from "../../services/advertisements/approvedAdvertisementFalseForAdmin";
import {approvedAdvertisementTrueForAdmin} from "../../services/advertisements/approvedAdvertisementTrueForAdmin";
import {deleteAdvertisementForAdmin} from "../../services/advertisements/deleteAdvertisementForAdmin";
import dayjs from "dayjs";
  import {
    useColorModeValue,
  } from "@/components/ui/color-mode"
  import { MdLocalShipping } from "react-icons/md";

  export default function AdvertisementCardFull({ ad, onEdit }) {
    const [currentPhotoIndex, setCurrentPhotoIndex] = useState(0);
    const { coordinates } = ad.adressCoordinates;
    const [role, setRole] = useState(null);
    const [id, setId] = useState(null);
    const [isAuthenticated, setIsAuthenticated] = useState(false);

    if (!Array.isArray(coordinates) || coordinates.length !== 2) {
      return <div>Ошибка: неверные координаты</div>;
    }
    const navigate = useNavigate();
    const imageHeight = useBreakpointValue({ base: "250px", md: "400px", lg: "500px" });
    const mapHeight = useBreakpointValue({ base: "250px", md: "300px", lg: "400px" });
    const mapWeight = useBreakpointValue({ base: "250px", md: "300px", lg: "400px" });
    const [isDialogOpen, setIsDialogOpen] = useState(false);
    const [errorMessages, setErrorMessages] = useState({});
    const [okMessage, setOkMessage] = useState("");
    const [error, setError] = useState("");
    const [latitude, longitude] = coordinates;
    useEffect(() => {
      const checkAuth = () => {
        const token = localStorage.getItem("authToken");
        console.log(token);
        setIsAuthenticated(!!token);
        if (token) {
          setId(getUserId());
          setRole(getUserRole());
        } else {
          setRole(null);
          etUserId(null);
        }
      };
  
      checkAuth();
    }, [location]);
      // Открытие диалога
  const openDialog = () => setIsDialogOpen(true);
  // Закрытие диалога
  const closeDialog = () => setIsDialogOpen(false);
  
    const nextPhoto = () => setCurrentPhotoIndex((prev) => (prev + 1) % ad.photos.length);
    const prevPhoto = () => setCurrentPhotoIndex((prev) => (prev - 1 + ad.photos.length) % ad.photos.length);
    const getRatingColor = (rating) => {
      if (rating >= 4.5) return "green";
      if (rating >= 3.5) return "yellow";
      if (rating >= 2) return "orange";
      if (rating > 0) return "red";
      return "gray";
    };
    return (
      <Container maxW="full"  p={4} className="min-h-screen bg-gray-100" bg="white" shadow="lg" rounded="lg" >
        <SimpleGrid columns={{ base: 1, lg: 2 }} spacing={6}>
        <Box position="relative" w="full" rounded="xl" bg="white" boxShadow="md" p={5} mb={6}>
  <VStack align="start" spacing={4}>
    {ad.confirmationStatus === true && (id?.toString() === ad.idAuthor?.toString() || role === "Admin") &&
    (<>
      <Text color="green">Размещение одобрено</Text>
    </>)}
    {ad.confirmationStatus === false && (id?.toString() === ad.idAuthor?.toString() || role === "Admin") &&
    (<>
      <Text color="red">В размещении отклонено</Text>
    </>)}
    
    <Heading size="2xl" color="gray.800">
      {ad.objectType} — {ad.adressName}
    </Heading>
    <Text fontSize="xl" color="blue.600" fontWeight="bold">
      {ad.rentalPrice} ₽ / сутки
    </Text>
    <Box position="relative" w="full">
      <Image
        rounded="xl"
        src={
          ad.photos[currentPhotoIndex]?.valuePhoto ||
          "https://upload.wikimedia.org/wikipedia/ru/b/b1/Виктор_Говорков._Нет%21_%28плакат%29.jpg"
        }
        w="full"
        h={imageHeight}
        objectFit="cover"
        cursor="pointer"
        onClick={openDialog}
        transition="0.3s ease"
        _hover={{ opacity: 0.95 }}
      />

      {ad.photos.length > 1 && (
        <>
                  <Button             
 aria-label="Previous photo"
 position="absolute"
 top="50%"
 left="4"
 transform="translateY(-50%)"
 onClick={prevPhoto}
 bg="blackAlpha.600"
 color="white"
 _hover={{ bg: "blackAlpha.700" }}
 rounded="full"
 size="sm">
          <FaChevronLeft />
          </Button>

          <Button             
 aria-label="Next photo"
 position="absolute"
 top="50%"
 right="4"
 transform="translateY(-50%)"
 onClick={nextPhoto}
 bg="blackAlpha.600"
 color="white"
 _hover={{ bg: "blackAlpha.700" }}
 rounded="full"
 size="sm">
          <FaChevronRight />
          </Button>
        </>
      )}
    </Box>
    <>
    <Flex justify="flex-end" align="center" w="100%">
  <Box
    display="flex"
    alignItems="center"
    gap={2}
  >
    <RatingGroup.Root
      name="Рейтинг"
      readOnly
      count={5}
      size="md"
      value={parseFloat(ad.rating.toFixed(1))}
      colorPalette={getRatingColor(ad.rating.toFixed(1))}
    >
      <RatingGroup.HiddenInput />
      <RatingGroup.Control />
    </RatingGroup.Root>
    <Text
      fontSize="lg"
      fontWeight="bold"
      color={getRatingColor(ad.rating.toFixed(1))}
    >
      {ad.rating.toFixed(1)}
    </Text>
  </Box>
</Flex>
    </>
    <Text>
  <b>Дата создания объявления:</b> {dayjs(ad.dateCreate).format("DD.MM.YYYY")}
</Text>
    <Heading size="md" color="gray.700">
      Информация об объявлении
    </Heading>
    <Text color="black" fontSize="md">
      {ad.description}
    </Text>
  </VStack>
</Box>
          <VStack align="start" spacing={4} p={6} >
            <Box w="full">
              <Heading size="2xl" color = "black" mb={2}>О недвижимости</Heading>
              <SimpleGrid columns={2} spacing={2}>
                <Text><b>Площадь:</b> {ad.totalArea} м²</Text>
                <Text><b>Комнат:</b> {ad.numberOfRooms}</Text>
                <Text><b>Спальных мест:</b> {ad.numberOfBeds}</Text>
                <Text><b>Санузлов:</b> {ad.numberOfBathrooms}</Text>
                <Text><b>Количество сдач в аренду:</b> {ad.numberOfTransactions}</Text>
              </SimpleGrid>
            </Box> 
            <Box>
              <Heading size="md" color = "black" mb={2}>Удобства</Heading>
              {ad.amenityes.length > 0 ? (
                <Wrap spacing={2}>
                  {ad.amenityes.map((amenity) => (
                    <WrapItem key={amenity.id}>
                      <Tag.Root size = "lg" color = "black" bg= "#FDD835"><Tag.Label>{amenity.amenityType}</Tag.Label></Tag.Root>
                    </WrapItem>
                  ))}
                </Wrap>
              ) : (
                <Text>Удобства не указаны</Text>
              )}
            </Box>
            <VStack align="start" spacing={6} w="100%">
  <Heading size="lg" color="gray.800">
    Расположение
  </Heading>

  <Box
    w="100%"
    h={mapHeight}
    bg="white"
    p={4}
    rounded="2xl"
    boxShadow="md"
    border="1px solid"
    borderColor="gray.200"
    display="flex"
    justifyContent="center"
    alignItems="center"
    position="relative"
  >
    <YMaps query={{ apikey: "5baeaca9-9934-42c3-bf93-ec536e4f87b2" }}>
      <Map
        defaultState={{ center: [longitude, latitude], zoom: 10 }}
        width="100%"
        height="100%"
      >
        <Placemark
          geometry={[longitude, latitude]}
          options={{ iconColor: "#3B82F6" }} // красивый синий
        />
      </Map>
    </YMaps>
  </Box>
</VStack>

<VStack spacing={4} w="100%" align="stretch">
  {/* Кнопка перехода к автору */}
  <Button
  px={4}
  py={2}
  rounded="lg"
  bg = "gray.200"
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
    onClick={() => navigate(`/user/${ad.idAuthor}`)}
  >
    Автор объявления
  </Button>
  <Button
        w="50%"
        size="lg"
        px={4}
        py={3}
        bg="#111111"
        color="white"
        fontWeight="semibold"
        rounded="lg"
        transition="all 0.3s ease"
        _hover={{
          bg: "green", // чуть темнее при наведении
          transform: "scale(1.04)",
          //color: "black",
          boxShadow: "0 6px 14px rgba(253, 216, 53, 0.35)"
        }}
        _active={{
          transform: "scale(0.98)",
          boxShadow: "0 2px 6px rgba(253, 216, 53, 0.2)"
        }}
        onClick={() => navigate(`/reviews/${ad.id}/1`)}
      >
        Отзывы
      </Button>

  {/* Кнопка "Забронировать" (если пользователь авторизован) */}
  {isAuthenticated && (
    <Button
      size="lg"
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
      onClick={() => navigate(`/createRentalRequest/${ad.id}`)}
    >
      Забронировать
    </Button>
  )}

  {/* Кнопки автора объявления */}
  {id?.toString() === ad.idAuthor?.toString() && (
    <>
      <HStack spacing={4}>
        <Button
                size="lg"
                bg="#111111"
                color="white"
                fontWeight="semibold"
                rounded="xl"
                transition="all 0.3s ease"
                _hover={{
                  bg: "red",
                  transform: "scale(1.04)",
                  color: "black",
                  boxShadow: "0 6px 14px rgba(253, 53, 53, 0.35)",
                }}
                _active={{
                  transform: "scale(0.98)",
                  boxShadow: "0 2px 6px rgba(250, 30, 22, 0.2)",
                }}
          w="50%"
          onClick={async () => {
            try {
              const result = await deleteAdvertisementForUser(ad.id);
              if (result.success) {
                console.log("Объявление удалено успешно");
                navigate("/MyAdvertisements/1");
              } else {
                console.error("Ошибка при удалении:", result.errors);
              }
            } catch (error) {
              console.error("Ошибка при удалении объявления:", error);
            }
          }}
        >
          Удалить
        </Button>
        <Button
                size="lg"
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
          w="50%"
          onClick={onEdit}
        >
          Редактировать
        </Button>
      </HStack>

      <Button
                        size="lg"
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
        w="100%"
        onClick={() => navigate(`/rentalRequests/${ad.id}/1`)}
      >
        Запросы на бронирование
      </Button>
    </>
  )}

  {/* Админские кнопки */}
  {role === "Admin" && (
    <>
    <Heading>Функции Админа</Heading>
    <VStack spacing={3}>
      <Button
                size="lg"
                bg="#111111"
                color="white"
                fontWeight="semibold"
                rounded="xl"
                transition="all 0.3s ease"
                _hover={{
                  bg: "red",
                  transform: "scale(1.04)",
                  color: "black",
                  boxShadow: "0 6px 14px rgba(253, 53, 53, 0.35)",
                }}
                _active={{
                  transform: "scale(0.98)",
                  boxShadow: "0 2px 6px rgba(250, 30, 22, 0.2)",
                }}
        w="100%"
        onClick={async () => {
          setErrorMessages({});
          setOkMessage("");
          let error = false;
          const result = await approvedAdvertisementFalseForAdmin(ad.id);
          if (result.success) {
            setOkMessage("Размещение отклонено!");
            console.log("Размещение отклонено");
          } else {
            console.error("Ошибка при отклонении:", result.errors);
            setErrorMessages(result.errors || { general: "Ошибка при отклонении" });
          }
        }}
      >
        Отклонить размещение
      </Button>
      <Button
                size="lg"
                bg="#111111"
                color="white"
                fontWeight="semibold"
                rounded="xl"
                transition="all 0.3s ease"
                _hover={{
                  bg: "green",
                  transform: "scale(1.04)",
                  color: "black",
                  boxShadow: "0 6px 14px rgba(7, 218, 25, 0.35)",
                }}
                _active={{
                  transform: "scale(0.98)",
                  boxShadow: "0 2px 6px rgba(36, 228, 11, 0.2)",
                }}
        w="100%"
        onClick={async () => {
          setErrorMessages({});
          setOkMessage("");
          let error = false;
          const result = await approvedAdvertisementTrueForAdmin(ad.id);
          if (result.success) {
            setOkMessage("Размещение одобрено!");
            setError(result.error);
            console.log("Размещение одобрено");
          } else {
            setErrorMessages(result.errors || { general: "Ошибка при одобрении" });
            setError(result.error);
            console.error("Ошибка при одобрении:", result.errors);
          } 
        }}
      >
        Одобрить размещение
      </Button>
      <Button
                size="lg"
                bg="#111111"
                color="white"
                fontWeight="semibold"
                rounded="xl"
                transition="all 0.3s ease"
                _hover={{
                  bg: "red",
                  transform: "scale(1.04)",
                  color: "black",
                  boxShadow: "0 6px 14px rgba(253, 53, 53, 0.35)",
                }}
                _active={{
                  transform: "scale(0.98)",
                  boxShadow: "0 2px 6px rgba(250, 30, 22, 0.2)",
                }}
        onClick={async () => {
          const result = await deleteAdvertisementForAdmin(ad.id);
          if (result.success) {
            console.log("Объявление удалено успешно");
          } else {
            console.error("Ошибка при удалении:", result.errors);
          }
        }}
      >
        Удалить объявление (Admin)
      </Button>
    </VStack>
    {okMessage && (
                      <Box mt={4} p={2} bg="green.100" borderRadius="md">
                        <Text color="green.700">{okMessage}</Text>
                          </Box>
                   )}
                      {error && (
                          <Box mt={4} p={2} bg="red.100" borderRadius="md">
                              <Text color="red.700">{error}</Text>
                          </Box>
                      )}
    </>
  )}
</VStack>
          </VStack>
        </SimpleGrid>
        <Dialog.Root open={isDialogOpen} onOpenChange={closeDialog}  size="lg">
        <Portal>
          <Dialog.Backdrop />
          <Dialog.Positioner>
            <Dialog.Content maxWidth="90%" maxHeight="80vh" overflow="hidden">
              <Dialog.Body pt="4">
                <Dialog.Title>Фото недвижимости</Dialog.Title>
                <Dialog.Description mb="4">
                  <Image 
                    src={ad.photos[currentPhotoIndex]?.valuePhoto || "https://upload.wikimedia.org/wikipedia/ru/b/b1/Виктор_Говорков._Нет%21_%28плакат%29.jpg"}
                    alt="Фото недвижимости"
                    width="100%"
                    objectFit="contain"  // Изменено на contain для корректного отображения
                    maxHeight="70vh"     // Ограничение высоты изображения
                  />
                </Dialog.Description>
              </Dialog.Body>
              <Dialog.CloseTrigger top="0" insetEnd="-12" asChild>
                <CloseButton bg="bg" size="sm" />
              </Dialog.CloseTrigger>
            </Dialog.Content>
          </Dialog.Positioner>
        </Portal>
      </Dialog.Root>
      </Container>
    );
  }
  