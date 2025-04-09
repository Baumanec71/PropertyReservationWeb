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
  
    return (


      <Container maxW="full" p={4} className="min-h-screen bg-gray-100" bg="white" shadow="lg" rounded="lg">
        <SimpleGrid columns={{ base: 1, lg: 2 }} spacing={6}>
          <Box position="relative">
            <Image
              rounded="md"
              src={ad.photos[currentPhotoIndex]?.valuePhoto || "https://upload.wikimedia.org/wikipedia/ru/b/b1/Виктор_Говорков._Нет%21_%28плакат%29.jpg"}
              w="full"
              h={imageHeight}
              objectFit="cover"
              cursor="pointer"
              onClick={openDialog}
            />
            {ad.photos.length > 1 && (
              <>
                <IconButton icon={<FaChevronLeft />} position="absolute" top="43%" left="2" transform="translateY(-50%)" onClick={prevPhoto} bg="gray.600" color="white" _hover={{ bg: "gray.600" }} />
                <IconButton icon={<FaChevronRight />} position="absolute" top="43%" right="2" transform="translateY(-50%)" onClick={nextPhoto} bg="gray.600" color="white" _hover={{ bg: "gray.600" }} />
              </>
            )}
            <Heading size="xl">Информация о объявлении:</Heading>
            <Text>{ad.description}</Text>
          </Box>
          <VStack align="start" spacing={4} p={6} >
            <Heading size="xl">{ad.objectType} - {ad.adressName}</Heading>
            <Text fontSize="2xl" color="gray.700">{ad.rentalPrice} ₽ / мес</Text>
            <Text fontSize="2xl" color="gray.700"><b>Предоплата:</b> {ad.fixedPrepaymentAmount} ₽</Text>
            <Box w="full">
              <Heading size="md" mb={2}>О недвижимости</Heading>
              <SimpleGrid columns={2} spacing={2}>
                <Text><b>Дата создания объявления:</b> {ad.dateCreate}</Text>
                <Text><b>Площадь:</b> {ad.totalArea} м²</Text>
                <Text><b>Комнат:</b> {ad.numberOfRooms}</Text>
                <Text><b>Спальных мест:</b> {ad.numberOfBeds}</Text>
                <Text><b>Санузлов:</b> {ad.numberOfBathrooms}</Text>
                <Text><b>Рейтинг:</b> ⭐ {ad.rating.toFixed(1)}</Text>
                <Text><b>Количество сдач в аренду:</b> {ad.numberOfTransactions}</Text>
              </SimpleGrid>
            </Box> 
            <Box>
              <Heading size="md" mb={2}>В квартире есть</Heading>
              {ad.amenityes.length > 0 ? (
                <Wrap spacing={2}>
                  {ad.amenityes.map((amenity) => (
                    <WrapItem key={amenity.id}>
                      <Tag.Root colorScheme="blue"><Tag.Label>{amenity.amenityType}</Tag.Label></Tag.Root>
                    </WrapItem>
                  ))}
                </Wrap>
              ) : (
                <Text>Удобства не указаны</Text>
              )}
            </Box>
            <Heading size="xl" >Расположение</Heading>
            <Box   
            mt={8}
            w={mapWeight}
            h={mapHeight}
            position="relative"
            display="flex"
            justifyContent="center"
            alignItems="center">
               <YMaps query={{ apikey: "5baeaca9-9934-42c3-bf93-ec536e4f87b2" }}>
                <Map defaultState={{ center: [longitude, latitude], zoom: 10 }} width={mapWeight} height={mapHeight}>
                  <Placemark geometry={[longitude, latitude]} options={{ iconColor: "blue" }} />
                </Map>
              </YMaps>
            </Box>

  
    <Button
      colorScheme="blue"
      w="100%"
      size="lg"
      onClick={() => navigate(`/user/${ad.idAuthor}`)}
    >
      Автор объявления
    </Button>
  {!isAuthenticated ? null : (
    <Box w="100%" >
      <Button 
      size="lg"
      colorScheme="blue"
      w="full"
      onClick={() => navigate(`/createRentalRequest/${ad.id}`)}
      >
        Забронировать
        </Button>

{id?.toString() === ad.idAuthor?.toString() && (
  <>
  <HStack mt={4}>
    <Button
      colorScheme="red"
      variant="solid"
      w="50%"
      _hover={{ bg: "red.500" }}
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
      Удалить объявление
    </Button>
    <Button
      variant="solid"
      colorScheme="green"
      _hover={{ bg: "green.600" }}
      w="50%"
      onClick={onEdit}
    >
      Редактировать
    </Button>
  </HStack>

  <HStack mt={4}>
    <Button
      colorScheme="blue"
      variant="solid"
      w="50%"
      _hover={{ bg: "blue.500" }}
      onClick={async () => {
        try {
          navigate(`/rentalRequests/${ad.id}/1`);
        } catch (error) {
          console.error("Ошибка при обработке запроса:", error);
        }
      }}
    >
      Посмотреть запросы на бронирование
    </Button>
  </HStack>
</>

)}
  {role === "Admin" && (
 <VStack >
 <Button mt={4}
   colorScheme="red"
   w="100%"
   size="lg"
   _hover={{ bg: "red.500" }}
   onClick={async () => {
     const result = await approvedAdvertisementFalseForAdmin(ad.id);
     if (result.success) {
       console.log("Размещение объявления отклонено");
     //  navigate("/MyAdvertisements/1");
     } else {
       console.error("Ошибка при удалении:", result.errors);
     }
   }}
 >
   Отклонить запрос в размещении
 </Button>
 <Button 
   colorScheme="green"
   w="100%"
   size="lg"
   _hover={{ bg: "green.600" }}
   onClick={async () => {
     const result = await approvedAdvertisementTrueForAdmin(ad.id);
     if (result.success) {
       console.log("Размещение объявления одобрено");
     //  navigate("/MyAdvertisements/1");
     } else {
       console.error("Ошибка при одобрении:", result.errors);
     }
   }}
 >
   Одобрить размещение
 </Button>
 <Button
      colorScheme="red"
      w="100%"
      size="lg"
      _hover={{ bg: "red.500" }}
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
                      )}
    </Box>
  )}
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
  