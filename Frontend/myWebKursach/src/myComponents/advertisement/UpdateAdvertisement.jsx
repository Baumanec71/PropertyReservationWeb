import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import {
  Box,
  Button,
  useBreakpointValue,
  Text,
  Heading,
  Input,
  Textarea,
  Fieldset,
  createListCollection,
  CheckboxGroup,
  Stack,
  Image, 
  Checkbox 
} from "@chakra-ui/react";
import { Field } from "@/components/ui/field";
import { YMaps, Map, Placemark, ZoomControl, GeolocationControl, SearchControl } from "@pbe/react-yandex-maps";
import { updateAdvertisement } from "../../services/advertisements/updateAdvertisement";
import { getCreateAdvertisementFormModel } from "../../services/advertisements/createAdvertisement";
//import { getCreateAdvertisementForm } from "../../services/advertisements/createAdvertisement";
import { LuFileImage, LuX } from "react-icons/lu";
//import { registration } from "../../services/advertisements/createAdvertisement";
import {
  SelectContent,
  SelectItem,
  SelectLabel,
  SelectRoot,
  SelectTrigger,
  SelectValueText
} from "@/components/ui/select"
import { getUserEmail } from "../../services/InfoJwt/getUserEmail";

export default function UpdateAdvertisement({ ad, id, onUpdate, onCancel }) {
   const scale = useBreakpointValue({
      base: "0em",
      sm: "30em",
      md: "48em",
      lg: "62em",
      xl: "80em",
      "2xl": "96em",
    });
    const [formData, setFormData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");
    const [description, setDescription] = useState("");
    // Контролируемый маркер: в нем храним только одну точку
    const [placemark, setPlacemark] = useState(null);
    const navigate = useNavigate();
    const [objectTypeLocal, setObjectTypeLocal] = useState("0");
    const [email, setEmail] = useState(null);
    const [selectedAmenities, setSelectedAmenities] = useState([]);
    const [photos, setPhotos] = useState([]);
    const [errorMessages, setErrorMessages] = useState({});
    const [okMessage, setOkMessage] = useState(""); // Строка для успешного сообщения
    
        const handleSubmit = async () => {
          setErrorMessages({});
          setOkMessage("");
          let error = false;
          if (!formData.rentalPrice || isNaN(formData.rentalPrice)) {
            setErrorMessages((prev) => ({
              ...prev,
              RentalPrice: "Укажите цену аренды",
            }));
            error = true;
          }
          
          if (!formData.fixedPrepaymentAmount || isNaN(formData.fixedPrepaymentAmount)) {
            setErrorMessages((prev) => ({
              ...prev,
              FixedPrepaymentAmount: "Укажите цену предоплаты",
            }));
            error = true;
          }
  
          if(error == true){
            return;
          }

          const result = await updateAdvertisement(formData, id);
          
          if (result.success) {
            setOkMessage("Объявление создано!");
          } else {
            setErrorMessages(result.errors || { general: "Ошибка при создании объявления" });
          }
        };
  
    useEffect(() => {
      async function fetchFormData() {
        setLoading(true);
        try {

          const result = await getCreateAdvertisementFormModel(id);
          if (result.success) {
            setFormData(result.data);
            if (Array.isArray(result.data.createPhotos)) {
              const newPhotos = [];
            
              result.data.createPhotos.forEach((photo) => {
                if (photo.valuePhoto.startsWith("data:image")) {
                  // Уже Base64 с префиксом, добавляем напрямую
                  newPhotos.push({
                    preview: photo.valuePhoto,
                    base64: photo.valuePhoto.split(",")[1],
                  });
                  console.log("Base64 изображение добавлено");
                } else {
                  try {
                    // Добавляем префикс, если его нет
                    const base64Data = photo.valuePhoto.replace(/\s/g, ""); // Удаляем пробелы
                    const imageType = "jpeg"; // Можно динамически определять тип
                    const fullBase64 = `data:image/${imageType};base64,${base64Data}`;           
                    const byteCharacters = atob(base64Data);
                    const byteNumbers = new Array(byteCharacters.length)
                      .fill(0)
                      .map((_, i) => byteCharacters.charCodeAt(i));
                    const byteArray = new Uint8Array(byteNumbers);
                    const blob = new Blob([byteArray], { type: `image/${imageType}` });
            
                    newPhotos.push({
                      file: blob,
                      preview: fullBase64,
                      base64: base64Data,
                    });
            
                  } catch (error) {
                    console.error("Ошибка при обработке Base64:", error);
                  }
                }
              });
            
              // Устанавливаем состояние сразу после завершения цикла
              setPhotos([...newPhotos]);
            }
              
            setDescription(result.data.description || "");
            setSelectedAmenities(result.data.createAdvertisementAmenities.map(a => a.amenity));

            const token = localStorage.getItem("authToken");
            if (token) {
              const userEmail = await getUserEmail();
              setEmail(userEmail);
              setFormData((prevData) => ({
                ...prevData,
                login: userEmail,
              }));
            } else {
              setEmail(null);
            }
          } else {
            setError(result.error);
          }
        } catch (error) {
          console.error("Ошибка при загрузке данных формы:", error);
          setError("Не удалось загрузить данные.");
        } finally {
          setLoading(false);
        }
      }
      
      fetchFormData();
    }, []);
  
  
    useEffect(() => {
      const handler = setTimeout(() => {
        setFormData((prevData) => ({
          ...prevData,
          description,
        }));
      }, 300); // Задержка в 300 мс
  
      return () => clearTimeout(handler);
    }, [description]);
  
    const handleChange = (e) => {
      const { name, value } = e.target;
      setFormData((prevData) => ({
        ...prevData,
        [name]: value,
      }));
    };
    const handlePhotosChange = (e) => {
      const selectedFiles = Array.from(e.target.files);
      selectedFiles.forEach((file) => {
        const reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onload = () => {
          const dataUrl = reader.result;
          setPhotos((prevPhotos) => {
            const updatedPhotos = [
              ...prevPhotos,
              { file, preview: dataUrl, base64: dataUrl.split(",")[1] },
            ];
            setFormData((prevData) => ({
              ...prevData,
              createPhotos: updatedPhotos.map((p) => ({ valuePhoto: p.base64 })),
            }));
            return updatedPhotos;
          });
        };
      });
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
  
    const handleDescriptionChange = (e) => {
      setDescription(e.target.value);
    };
  
  
    // Обработчик клика по карте: обновляем контролируемый маркер и получаем адрес через геокодирование
    const handleMapClick = (event) => {
      const coords = event.get("coords");
      setPlacemark({
        latitude: coords[0],
        longitude: coords[1],
      });
      if (window.ymaps) {
        window.ymaps.geocode(coords).then((res) => {
          const firstGeoObject = res.geoObjects.get(0);
          const address = firstGeoObject.getAddressLine();
          setFormData((prevData) => ({
            ...prevData,
            latitude: coords[0],
            longitude: coords[1],
            adressName: address,
          }));
        });
      } else {
        setFormData((prevData) => ({
          ...prevData,
          latitude: coords[0],
          longitude: coords[1],
          adressName: "",
        }));
      }
    };
  
    // Обработчик выбора результата поиска: обновляем контролируемый маркер и данные формы
    const handleSearchResult = (event) => { 
      const index = event.get("index");
      const searchControl = event.get("target");
      let geoObject;
    
      // Получаем массив всех результатов
      const results = searchControl.getResultsArray();
    
      if (typeof index === "number" && results[index]) {
        geoObject = results[index];
      } else if (results.length > 0) {
        geoObject = results[0];
      }
    
      if (geoObject && geoObject.geometry) {
        const coords = geoObject.geometry.getCoordinates();
        const address = geoObject.getAddressLine();
    
        console.log("Coordinates from search:", coords);
        console.log("Address from search:", address);
    
        // Обновляем контролируемый маркер
        setPlacemark({
          latitude: coords[0],
          longitude: coords[1],
        });
    
        // Обновляем данные формы
        setFormData((prevData) => ({
          ...prevData,
          latitude: coords[0],
          longitude: coords[1],
          adressName: address,
        }));
      } else {
        console.error("GeoObject not found or invalid data.");
      }
    };
    
  
  
    const requiresInput = (amenity) => ![2, 3].includes(amenity.amenity);
  
    const handleCheckboxChange = (amenity, checked) => {
      console.log(amenity)
      console.log(checked)
      setFormData((prev) => ({
        ...prev,
        createAdvertisementAmenities: prev.createAdvertisementAmenities.map((a) =>
          a.amenity === amenity.amenity
            ? {
                ...a,
                isActive: checked,
                value: checked && ![2, 3].includes(a.amenity) ? 1 : a.value,
              }
            : a
        ),
      }));
      setSelectedAmenities(prev => checked ? [...prev, amenity.amenity] : prev.filter(a => a !== amenity.amenity));
    };

  
    const handleObjectTypeChange = (newVal) => {
      // Ставим локальное состояние, чтобы галочка обновилась
      {console.log(newVal)}
      setObjectTypeLocal(newVal.value);
  
      // Ставим в formData, чтобы условный рендер работал
      setFormData((prev) => ({
        ...prev,
        objectType: Number(newVal.value),
      }));
    };
  
    if (loading) {
      return (
        <Box textAlign="center" mt={8}>
          <Text>Загрузка формы...</Text>
        </Box>
      );
    }
  
    if (error && !formData) {
      return (
        <Box textAlign="center" mt={8}>
          <Text color="red.500">Ошибка: {error}</Text>
        </Box>
      );
    }
  
    
    const objectTypeCollection = createListCollection({
      items: Array.isArray(formData.types)
        ? formData.types.map((option) => ({
            label: option.displayName,
            value: String(option.value),
          }))
        : [
            { label: "Квартира", value: "0" },
            { label: "Дом", value: "1" },
          ],
    });

  return (
     <Box
       transform={`scale(${scale})`}
       transition="transform 0.2s ease-in-out"
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
         Создание объявления
       </Heading>
       <Text mb={4} textAlign="center">
         Заполните форму для создания объявления
       </Text>
 
       <Fieldset.Root size="lg" maxW="md">
         <Fieldset.Legend>Детали объявления</Fieldset.Legend>
         <Fieldset.HelperText>
           Пожалуйста, заполните данные для создания объявления.
         </Fieldset.HelperText>
 
         <Fieldset.Content>
           <Field label="Адрес">
             <Input
               name="adressName"
               type="text"
               value={formData.adressName || ""}
               readOnly
             />
             {errorMessages.AdressName && <Text color="red.500" mt={2}>{errorMessages.AdressName}</Text>}
           </Field>
           <Field label="Координаты">
             <Text mb={2}>
               Широта: {formData.latitude} | Долгота: {formData.longitude}
             </Text>
             <YMaps query={{ apikey: "5baeaca9-9934-42c3-bf93-ec536e4f87b2" }}>
              {console.log(formData.latitude)}
               <Map  
                 defaultState={{
                   center: [formData.latitude || 55.751574, formData.longitude || 37.573856],
                   zoom: 10,
                 }}
                 width="100%"
                 height="300px"
                 onClick={handleMapClick}
               >
                 {/* Используем noPlacemark: true, чтобы контролировать метку через состояние */}
                 <SearchControl
                   options={{ float: "right", noPlacemark: true }}
                   onResultSelect={handleSearchResult}
                 />
                 <GeolocationControl options={{ float: "left" }} />
                 <ZoomControl options={{ float: "right" }} />
                 {placemark ? ( 
  <Placemark
    geometry={[placemark.latitude, placemark.longitude]}
    options={{ iconColor: "blue" }}
  />
) : (
  <Placemark
    geometry={[formData.latitude, formData.longitude]}
    options={{ iconColor: "blue" }}
  />
)}
               </Map>
             </YMaps>
           </Field>
           <Field>
             <SelectRoot
               collection={objectTypeCollection}
               value={objectTypeLocal}
               onValueChange={handleObjectTypeChange}
               size="sm"
               width="320px"
             >
               <SelectLabel>Тип объекта</SelectLabel>
               <SelectTrigger>
                 <SelectValueText placeholder="Выберите тип объекта" />
               </SelectTrigger>
               <SelectContent>
                 {objectTypeCollection.items.map((item) => (
                   <SelectItem key={item.value} item={item}>
                     {item.label}
                   </SelectItem>
                 ))}
               </SelectContent>
             </SelectRoot>
           </Field>
             {Number(formData.objectType) === 0 && (
             <Field label="Номер квартиры">
               <Input
                 name="apartmentNumber"
                 type="text"
                 value={formData.apartmentNumber || 0}
                 onChange={(e) => {
                   let val = parseInt(e.target.value, 10);
             
                   if (isNaN(val)) val = 0; // Если NaN, ставим 0
                   if (val < 0) val = 0; // Запрещаем отрицательные числа
             
                   setFormData({ ...formData, apartmentNumber: val });
                 }}
               />
                {formData.apartmentNumber === "" && (
       <Text color="red.500" mt={2}>
         Введите номер квартиры
       </Text>
     )}
             </Field>
           )}
       <Field label="Описание">
         <Textarea
           name="description"
           value={description}
           onChange={handleDescriptionChange}
         />
         {errorMessages.Description && <Text color="red.500" mt={2}>{errorMessages.Description}</Text>}
       </Field>
       <Field label="Общая площадь">
   <Input
     name="totalArea"
     type="number"
     value={formData.totalArea || 0}
     onChange={(e) => {
       let val = parseInt(e.target.value, 10);
 
       if (isNaN(val)) val = 0; // Если NaN, ставим 0
       if (val < 0) val = 0; // Запрещаем отрицательные числа
 
       setFormData({ ...formData, totalArea: val });
     }}
   />
   {errorMessages.TotalArea && <Text color="red.500" mt={2}>{errorMessages.TotalArea}</Text>}
 </Field>
           <Field label="Арендная плата">
             <Input
               name="rentalPrice"
               type="number"
               value={formData.rentalPrice || 0}
               onChange={(e) => {
                 let val = parseInt(e.target.value, 10);
           
                 if (isNaN(val)) val = 0; // Если NaN, ставим 0
                 if (val < 0) val = 0; // Запрещаем отрицательные числа
           
                 setFormData({ ...formData, rentalPrice: val });
               }}
             />
             {errorMessages.RentalPrice && <Text color="red.500" mt={2}>{errorMessages.RentalPrice}</Text>}
           </Field>
           <Field label="Предоплата">
             <Input
               name="fixedPrepaymentAmount"
               type="number"
               value={formData.fixedPrepaymentAmount || 0}
               onChange={(e) => {
                 let val = parseInt(e.target.value, 10);
           
                 if (isNaN(val)) val = 0; // Если NaN, ставим 0
                 if (val < 0) val = 0; // Запрещаем отрицательные числа
           
                 setFormData({ ...formData, fixedPrepaymentAmount: val });
               }}
             />
              {errorMessages.FixedPrepaymentAmount && <Text color="red.500" mt={2}>{errorMessages.FixedPrepaymentAmount}</Text>}
           </Field>
           <Field label="Число комнат">
             <Input
               name="numberOfRooms"
               type="number"
               value={formData.numberOfRooms || 0}
               onChange={(e) => {
                 let val = parseInt(e.target.value, 10);
           
                 if (isNaN(val)) val = 0; // Если NaN, ставим 0
                 if (val < 0) val = 0; // Запрещаем отрицательные числа
           
                 setFormData({ ...formData, numberOfRooms: val });
               }}
             />
               {errorMessages.NumberOfRooms && <Text color="red.500" mt={2}>{errorMessages.NumberOfRooms}</Text>}
           </Field>
           <Field label="Количество спальных мест">
             <Input
               name="numberOfBeds"
               type="number"
               value={formData.numberOfBeds|| 0}
               onChange={(e) => {
                 let val = parseInt(e.target.value, 10);
           
                 if (isNaN(val)) val = 0; // Если NaN, ставим 0
                 if (val < 0) val = 0; // Запрещаем отрицательные числа
           
                 setFormData({ ...formData, numberOfBeds: val });
               }}
             />
             {errorMessages.NumberOfBeds && <Text color="red.500" mt={2}>{errorMessages.NumberOfBeds}</Text>}
           </Field>
           <Field label="Количество санузлов">
             <Input
               name="numberOfBathrooms"
               type="number"
               value={formData.numberOfBathrooms || 0}
               onChange={(e) => {
                 let val = parseInt(e.target.value, 10);
           
                 if (isNaN(val)) val = 0; // Если NaN, ставим 0
                 if (val < 0) val = 0; // Запрещаем отрицательные числа
           
                 setFormData({ ...formData, numberOfBathrooms: val });
               }}
             />
              {errorMessages.NumberOfBathrooms && <Text color="red.500" mt={2}>{errorMessages.NumberOfBathrooms}</Text>}
           </Field>
           <Field label="Фото">
             <Box position="relative" mb={4}>
               {photos.length > 0 && (
                 <Stack direction="row" spacing={2}>
                   {photos.map((photo, index) => (
                     <Box key={index} position="relative">
                       <Image
                         src={photo.preview}
                         alt={`Фото ${index + 1}`}
                         boxSize="100px"
                         objectFit="cover"
                         borderRadius="md"
                       />
                       <Button
                         position="absolute"
                         top="-5px"
                         right="-5px"
                         size="xs"
                         colorScheme="red"
                         borderRadius="full"
                         onClick={() => removePhoto(index)}
                       >
                         <LuX size={14} />
                       </Button>
                     </Box>
                   ))}
                 </Stack>
               )}
               <Button mt={2} onClick={() => document.getElementById("photosInput").click()}>
                 <LuFileImage /> Загрузить фото
               </Button>
               <Input
                 type="file"
                 id="photosInput"
                 multiple
                 accept="image/*"
                 hidden
                 onChange={handlePhotosChange}
               />
             </Box>
           </Field>
           <CheckboxGroup name="Удобства">
   <Fieldset.Content>
     <Stack direction="column" spacing={3}>
       {formData.createAdvertisementAmenities.map((amenity) => (
         <Box key={amenity.amenity}>
           {console.log(`Amenity: ${amenity.amenity}, Active: ${amenity.isActive}`)}
           <Checkbox.Root
             value={amenity.amenity}
             checked={amenity.isActive || false}
             onCheckedChange={(e) => handleCheckboxChange(amenity, !!e.checked)}
           >
             <Checkbox.HiddenInput />
             <Checkbox.Control />
             <Checkbox.Label>{amenity.amenityDisplay}</Checkbox.Label>
           </Checkbox.Root>
           {amenity.isActive && requiresInput(amenity) && (
             <Input
               mt={2}
               type="number"
               placeholder={`Введите количество для ${amenity.amenityDisplay}`}
               value={amenity.value || 1}
               onChange={(e) => {
                 const newValue = parseInt(e.target.value, 10) || 0;
                 setFormData((prev) => ({
                   ...prev,
                   createAdvertisementAmenities: prev.createAdvertisementAmenities.map((a) =>
                     a.amenity === amenity.amenity ? { ...a, value: newValue } : a
                   ),
                 }));
               }}
             />
           )}
         </Box>
       ))}
     </Stack>
   </Fieldset.Content>
 </CheckboxGroup>
         </Fieldset.Content>
                         {okMessage && (
                             <Box mt={4} p={2} bg="green.100" borderRadius="md">
                                 <Text color="green.700">{okMessage}</Text>
                             </Box>
                         )}
         
                         {Object.keys(errorMessages).map((field, index) => (
                             <Box key={index}>
                               {console.log(errorMessages)}
                                 {(Array.isArray(errorMessages[field]) ? errorMessages[field] : [errorMessages[field]]).map((error, idx) => (
                                     <Text key={idx} color="red.500" fontSize="sm">
                                         {error}
                                     </Text>
                                 ))}
                             </Box>
                         ))}
       </Fieldset.Root>
 
       {error && (
         <Box mt={4} p={2} bg="red.100" borderRadius="md">
           <Text color="red.700">{error}</Text>
         </Box>
       )}
 
 <Box display="flex" gap={2} mt={4} w="full">
        <Button colorScheme="red" flex={1} onClick={onCancel}>
          Назад
        </Button>
        <Button colorScheme="blue" flex={1} onClick={handleSubmit}>
          Применить
        </Button>
      </Box>
     </Box>
   );
 }
 
 