import { Card, Stack, Text, Button, Image,HStack, IconButton,Heading,Flex , Grid, GridItem, Tag, Box,  Collapsible,
    RatingGroup,  } from "@chakra-ui/react";
import { useNavigate } from "react-router-dom";
import { BiLogoBaidu, BiLoaderCircle} from "react-icons/bi";
import { FaChevronLeft, FaChevronRight } from 'react-icons/fa6'
import { useState, useEffect } from "react";
import { getUserRole } from "../../services/InfoJwt/getUserRole";
import {deleteAdvertisementForAdmin} from "../../services/advertisements/deleteAdvertisementForAdmin";

export default function AdvertisementCard({ ad }) {
  console.log(ad)
    const navigate = useNavigate();
    const [currentPhotoIndex, setCurrentPhotoIndex] = useState(0);
    const [role, setRole] = useState(null);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
    const nextPhoto = () => setCurrentPhotoIndex((prevIndex) => (prevIndex + 1) % ad.photos.length);
    const prevPhoto = () => setCurrentPhotoIndex((prevIndex) => (prevIndex - 1 + ad.photos.length) % ad.photos.length);

   useEffect(() => {
      const checkAuth = () => {
        const token = localStorage.getItem("authToken");
        console.log(token);
        setIsAuthenticated(!!token);
        if (token) {
          setRole(getUserRole());
        } else {
          setRole(null);

        }
      };
  
      checkAuth();
    }, [location]);

    return (
      <Card.Root className="w-full md:w-96 bg-white shadow-xl rounded-2xl overflow-hidden transition-all transform hover:scale-105 hover:shadow-2xl p-5">
<Card.Header>
  <Box position="relative" w="full" h="400px" className="rounded-xl overflow-hidden">
    {ad.photos.length > 0 ? (
      <Image
        src={ad.photos[currentPhotoIndex].valuePhoto}
        alt="Фото"
        objectFit="cover"
        w="full"
        h="full"
        className="transition-all duration-300 ease-in-out"
      />
    ) : ( 
      <Image
        src="https://upload.wikimedia.org/wikipedia/ru/b/b1/Виктор_Говорков._Нет%21_%28плакат%29.jpg"
        alt="Фото по умолчанию"
        objectFit="cover"
        w="full"
        h="full"
        className="transition-all duration-300 ease-in-out"
      />
    )}
    {ad.photos.length > 1 && (
      <>
        <IconButton
          icon={<FaChevronLeft />}
          position="absolute"
          top="50%"
          left="2"
          transform="translateY(-50%)"
          onClick={prevPhoto}
          size="sm"
          variant="solid"
          aria-label="Предыдущее фото"
          className="bg-gray-800 text-white hover:bg-gray-600 transition-all"
        />
        <IconButton
          icon={<FaChevronRight />}
          position="absolute"
          top="50%"
          right="2"
          transform="translateY(-50%)"
          onClick={nextPhoto}
          size="sm"
          variant="solid"
          aria-label="Следующее фото"
          className="bg-gray-800 text-white hover:bg-gray-600 transition-all"
        />
      </>
    )}
  </Box>
          <Heading size="md" className="text-gray-800 font-bold mt-3">{ad.objectType}</Heading>
        </Card.Header>
        <Card.Body>
          <Stack spacing={3}>
            <Text className="text-gray-600"><b>Адрес:</b> {ad.adressName}</Text>
            <Text><b>Площадь:</b> {ad.totalArea} м²</Text>
            <Text><b>Комнат:</b> {ad.numberOfRooms}, <b>Спальных мест:</b> {ad.numberOfBeds}</Text>
            <Text><b>Санузлы:</b> {ad.numberOfBathrooms}</Text>
            <Text className="text-lg font-semibold text-green-600">Аренда: {ad.rentalPrice} ₽</Text>
            <Text className="text-md text-red-500">Предоплата: {ad.FixedPrepaymentAmount} ₽</Text>
  
            <Flex align="center" justify="flex-start">
              <Text fontWeight="bold">Рейтинг:</Text>
              <Box ml={2} className="text-yellow-400 text-lg">⭐ {ad.rating.toFixed(1)}</Box>
            </Flex>
  
            <Text fontWeight="bold">Удобства:</Text>
            <Grid templateColumns={{ base: "repeat(2, 1fr)", md: "repeat(3, 1fr)" }} gap={2}>
              {ad.amenityes && ad.amenityes.length > 0
                ? ad.amenityes.map((amenity) => (
                    <GridItem key={amenity.id}>
                      <Tag.Root className="bg-blue-500 text-white px-2 py-1 rounded-lg">
                        <Tag.Label>{amenity.amenityType}</Tag.Label>
                      </Tag.Root>
                    </GridItem>
                  ))
                : <Text className="text-gray-500">Не указаны</Text>
              }
            </Grid>
  
            <Text fontWeight="bold" mt={2}>Описание:</Text>
            <Text className="text-gray-700">{ad.description.slice(0, 100)}{ad.description.length > 100 && '...'}</Text>
          </Stack>
        </Card.Body>
        <Card.Footer display="flex" flexDirection="column" gap={2}>
  <HStack w="100%" spacing={4}>
    <Button
      colorScheme="blue"
      w="30%"
      size="lg"
      onClick={() => navigate(`/advertisement/${ad.id}`)}
    >
      Подробнее
    </Button>
    <Button
      colorScheme="blue"
      w="70%"
      size="lg"
      onClick={() => navigate(`/user/${ad.idAuthor}`)}
    >
      Автор объявления
    </Button>
  </HStack>

  {!isAuthenticated ? null : (
    <Box w="100%">
      {role === "Admin" && (
        <Button
             colorScheme="red"
             w="100%"
             size="lg"
             _hover={{ bg: "red.500" }}
             onClick={async () => {
               const result = await deleteAdvertisementForAdmin(ad.id);
               if (result.success) {
                 console.log("Объявление удалено успешно");
                // navigate("/MyAdvertisements/1");
               } else {
                 console.error("Ошибка при удалении:", result.errors);
               }
             }}
           >
             Удалить объявление
           </Button>
      )}
    </Box>
  )}
</Card.Footer>
      </Card.Root>
    );
  }