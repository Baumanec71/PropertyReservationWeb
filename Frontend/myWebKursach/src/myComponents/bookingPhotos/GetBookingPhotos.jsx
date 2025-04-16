import { useEffect, useState } from "react";
import {
  Card,
  CardHeader,
  CardBody,
  CardFooter,
  Image,
  Box,
  Heading,
  Text,
  Button,
  HStack,
  Tag,
  Select, 
  Grid,
  GridItem, Input, Stack, VStack, IconButton, CloseButton, Dialog, Portal, Container, Drawer  
} from "@chakra-ui/react";
import { FaChevronLeft, FaChevronRight } from "react-icons/fa6";
import { LuX } from "react-icons/lu";
import { getBookingPhotos } from "../../services/bookingPhotos/getBookingPhotos";
import { Field } from "@/components/ui/field";
import { LuFileImage } from "react-icons/lu";
import { useParams } from "react-router-dom";
import { addBookingPhotos } from "../../services/bookingPhotos/addBookingPhotos";
import * as exifr from 'exifr';


export default function GetBookingPhotos() {
    const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [currentPhotoIndex, setCurrentPhotoIndex] = useState(0);
  const [dialogPhotoSet, setDialogPhotoSet] = useState([]);
  const [beforePhotos, setBeforePhotos] = useState([]);
  const [afterPhotos, setAfterPhotos] = useState([]);
  const [photos, setPhotos] = useState([]);
  const [beforeIndex, setBeforeIndex] = useState(0);
  const [afterIndex, setAfterIndex] = useState(0);
  const [formData, setFormData] = useState({ createPhotos: [] });
  const { rentalRequestId } = useParams();

  useEffect(() => {
    const fetchPhotos = async () => {
      try {
        const result = await getBookingPhotos(rentalRequestId);
        const before = result?.data?.data?.item1 || [];
        const after = result?.data?.data?.item2 || [];
        setBeforePhotos(before);
        setAfterPhotos(after);
        setBeforeIndex(0);
        setAfterIndex(0);
      } catch (error) {
        console.error("Error fetching photos:", error);
      }
    };

    fetchPhotos();
  }, [rentalRequestId]);

  const handlePhotosChange = async (e) => {
    const selectedFiles = Array.from(e.target.files);
    const now = new Date();
  
    for (const file of selectedFiles) {
      try {
        const metadata = await exifr.parse(file, { tiff: true, ifd0: true });
        const photoTime = metadata?.DateTimeOriginal || metadata?.CreateDate;
  
        if (!photoTime) {
          alert("Фотография не содержит данных о времени съёмки. Пожалуйста, сделайте новое фото.");
          continue;
        }
  
        const photoDate = new Date(photoTime);
        const diff = Math.abs(now.getTime() - photoDate.getTime());
  
        // допустим максимум 2 минуты разницы
        if (diff > 2 * 60 * 1000) {
          alert("Фото слишком старое. Пожалуйста, сделайте новое.");
          continue;
        }
  
        const reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onload = () => {
          const dataUrl = reader.result;
          setPhotos((prevPhotos) => {
            const updatedPhotos = [
              ...prevPhotos,
              {
                file,
                preview: dataUrl,
                base64: dataUrl.split(",")[1],
                dateCreate: new Date().toISOString(),
              },
            ];
            setFormData((prevData) => ({
              ...prevData,
              createPhotos: updatedPhotos.map((p) => ({
                valuePhoto: p.base64,
                dateCreate: p.dateCreate,
                deleteStatus: false,
              })),
            }));
            return updatedPhotos;
          });
        };
      } catch (err) {
        console.error("Ошибка при чтении EXIF:", err);
        alert("Не удалось прочитать данные о фото. Пожалуйста, сделайте новое фото.");
      }
    }
  };

  const removePhoto = (index) => {
    setPhotos((prevPhotos) => {
      const updatedPhotos = prevPhotos.filter((_, i) => i !== index);
      setFormData((prevData) => ({
        ...prevData,
        createPhotos: updatedPhotos.map((p) => ({ valuePhoto: p.base64 })),
      }));
      return updatedPhotos;
    });
  };

  const uploadPhotos = async () => {
    const payload = await Promise.all(
      photos.map(
        (p) =>
          new Promise((resolve, reject) => {
            const reader = new FileReader();
            reader.onload = () => {
              resolve({
                valuePhoto: reader.result.split(",")[1],
                dateCreate: new Date().toISOString(),
                deleteStatus: false,
              });
            };
            reader.onerror = reject;
            reader.readAsDataURL(p.file);
          })
      )
    );

    const result = await addBookingPhotos(payload, rentalRequestId);
    if (result?.success) {
      setPhotos([]);
      alert("Фотографии успешно загружены");
    } else {
      alert("Ошибка загрузки: " + result?.error);
    }
  };

  const openDialogWithPhoto = (photos, index) => {
    setDialogPhotoSet(photos);
    setCurrentPhotoIndex(index);
    setIsDialogOpen(true);
  };
  
  const closeDialog = () => setIsDialogOpen(false);

  const renderMainPhotoBlock = (photos, index, setIndex, label) => {
    {console.log(photos[index])}
    const photoDate = photos[index]?.dateCreate
      ? new Date(photos[index].dateCreate).toLocaleString("ru-RU", {
          day: "2-digit",
          month: "2-digit",
          year: "numeric",
          hour: "2-digit",
          minute: "2-digit",
          second: "2-digit",
        })
      : null;
  
    return (
      
      <Box>
        <Heading size="sm" mb={2}>{label}</Heading>
        {photos.length > 0 ? (
          <VStack>
            <Box position="relative">
              <Image
                onClick={() => openDialogWithPhoto(photos, index)}
                src={photos[index]?.valuePhoto}
                alt={`${label} - главное фото`}
                maxHeight="400px"
                objectFit="contain"
                borderRadius="md"
                boxShadow="md"
              />
              <HStack
                position="absolute"
                top="50%"
                left="0"
                right="0"
                justify="space-between"
                px={2}
              >
                <IconButton
                  icon={<FaChevronLeft />}
                  aria-label="Предыдущее фото"
                  onClick={() =>
                    setIndex((prev) => (prev > 0 ? prev - 1 : photos.length - 1))
                  }
                  variant="ghost"
                />
                <IconButton
                  icon={<FaChevronRight />}
                  aria-label="Следующее фото"
                  onClick={() =>
                    setIndex((prev) => (prev < photos.length - 1 ? prev + 1 : 0))
                  }
                  variant="ghost"
                />
              </HStack>
            </Box>

            {photoDate && (
              <Text mt={1} color="gray.600" fontSize="sm">
                Дата съёмки: {photoDate}
              </Text>
            )}
  
            <HStack mt={2} wrap="wrap">
              {photos.map((photo, i) => (
                <Image
                  key={photo.id || i}
                  src={photo.valuePhoto}
                  alt={`thumb-${i}`}
                  boxSize="60px"
                  objectFit="cover"
                  border={i === index ? "2px solid #FDD835" : "1px solid gray"}
                  borderRadius="md"
                  cursor="pointer"
                  onClick={() => setIndex(i)}
                />
              ))}
            </HStack>
          </VStack>
        ) : (
          <Text color="gray.500">Нет фото</Text>
        )}
      </Box>
    );
  };

  return (
    <Container position = "relative" mt = "4" h = "100%" w = "100%">
    <Card.Root bg="white" overflow="hidden" rounded="2xl" shadow="md" borderWidth="1px">
      <Card.Header>
        <Heading size="md">Фото до и после аренды</Heading>
      </Card.Header>

      <Card.Body>
        <Grid templateColumns={{ base: "1fr", md: "1fr 1fr" }} gap={4}>
          {renderMainPhotoBlock(beforePhotos, beforeIndex, setBeforeIndex, "До аренды")}
          {renderMainPhotoBlock(afterPhotos, afterIndex, setAfterIndex, "После аренды")}
        </Grid>

        <Field label="Фото">
          <Box position="relative" mb={4}>
            {photos.length > 0 && (
              <Stack direction="row" spacing={2} flexWrap="wrap">
                {photos.map((photo, index) => (
                  <Box key={index} position="relative">
                    <Image
                      src={photo.preview}
                      alt={`Фото ${index + 1}`}
                      boxSize="200px"
                      objectFit="cover"
                      borderRadius="md"
                    />
                    <Button
                      position="absolute"
                      top="-5px"
                      right="-5px"
                      size="xs"
                      bg="black"
                      color="red"
                      variant="outline"
                      _hover={{
                        bg: "red",
                        color: "black",
                        transform: "scale(1.03)",
                      }}
                      onClick={() => removePhoto(index)}
                    >
                      <LuX size={14} />
                    </Button>
                  </Box>
                ))}
              </Stack>
            )}

            <Button
              width="full"
              size="lg"
              px={6}
              py={3}
              mt={4}
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
              onClick={() => document.getElementById("photosInput").click()}
              leftIcon={<LuFileImage />}
            >
              Загрузить фото
            </Button>

            <Input
              type="file"
              id="photosInput"
              multiple
              accept="image/*"
              capture="environment"
              hidden
              onChange={handlePhotosChange}
            />

            {photos.length > 0 && (
              <Button mt={4} colorScheme="yellow" onClick={uploadPhotos}>
                Отправить фотографии
              </Button>
            )}
          </Box>
        </Field>
      </Card.Body>

      <Card.Footer>
        <Text color="gray.600" fontSize="sm">
          Всего фото: до — {beforePhotos.length}, после — {afterPhotos.length}
        </Text>
      </Card.Footer>
    </Card.Root>
    <Dialog.Root open={isDialogOpen} onOpenChange={closeDialog} size = "full" >
    <Portal>
      <Dialog.Backdrop />
      <Dialog.Positioner mt ="4" >
        <Dialog.Content overflow="hidden" >
          <Dialog.Body size = "full">
            <Dialog.Description mb="4">
              <Image 
                htmlWidth="100%"
                htmlHeight="100%"
                src={dialogPhotoSet[currentPhotoIndex]?.valuePhoto || "https://upload.wikimedia.org/wikipedia/ru/b/b1/Виктор_Говорков._Нет%21_%28плакат%29.jpg"}
                alt="Фото недвижимости"
                objectFit="contain"
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