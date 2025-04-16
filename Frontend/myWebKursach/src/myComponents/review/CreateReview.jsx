import {
    Box,
    Button,
    Heading,
    Input,
    Text,
    useBreakpointValue,
    Fieldset,
    RatingGroup, Textarea
} from "@chakra-ui/react";
import { useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { createReview } from "../../services/reviews/createReview"; // Убедись в правильном пути
import { Field } from "@/components/ui/field";


export default function CreateReview() {
    const scale = useBreakpointValue({
        base: "0em",
        sm: "30em",
        md: "48em",
        lg: "62em",
        xl: "80em",
        "2xl": "96em"
    });

    const [rating, setRating] = useState(0);
    const [comment, setComment] = useState("");
    //const [rentalId, setRentalId] = useState("");
    const [errors, setErrors] = useState([]);
    const [okMessage, setOkMessage] = useState("");
    const [isLoading, setIsLoading] = useState(false);
    const navigate = useNavigate();
     const { idNeedRentalRequest } = useParams();

    const handleSubmit = async () => {
        if (!idNeedRentalRequest) {
            console.log("ID запроса не найдено");
            setError("ID запроса не найдено.");
            return;
        }
        setErrors([]);
        setOkMessage("");
        setIsLoading(true);

        const reviewData = {
            theQualityOfTheTransaction: Number(rating),
            comment: comment,
            idRental: idNeedRentalRequest
        };


        const result = await createReview(reviewData);

        setIsLoading(false);

        if (!result.success) {
            setErrors(result.errors || ["Произошла неизвестная ошибка."]);
            alert(result.errors)
        } else {
            alert(result.data)
            setOkMessage("Отзыв успешно отправлен!");
            setRating(0);
            setComment("");
            setRentalId("");
        }
    };
    const color = "black";
    const bg = `${BG}`;
    return (
<Box
  transform={`scale(${scale})`}
  transition="transform 0.2s ease-in-out"
  position="relative"
  maxW="2xl"
  w="full"
  px={8}
  py={10}
  bg="white"
  borderRadius="2xl"
  boxShadow="2xl"
  mx="auto"
  mt={12}
>
  <Heading as="h2" size="xl" textAlign="center" mb={6} color="gray.800">
    Оставьте отзыв
  </Heading>

  <Box display="flex" justifyContent="center" mb={6}>
    <RatingGroup.Root
      name="selectedMinRating"
      placeholder="Поставьте оценку"
      count={5}
      size="lg"
      value={rating ?? 0}
      colorPalette="yellow"
      defaultValue={0}
      onValueChange={(newValue) => {
        const rating = newValue?.value ?? 0;
        setRating(rating);
      }}
    >
      <RatingGroup.HiddenInput />
      <RatingGroup.Control />
    </RatingGroup.Root>
  </Box>

  <Fieldset.Root size="lg" maxW="lg" mx="auto">
    <Fieldset.Content>
      <Field label="Комментарий">
        <Textarea
          type="text"
          name="comment"
          value={comment}
          onChange={(e) => setComment(e.target.value)}
          placeholder="Напишите, что вам понравилось или что можно улучшить"
          size="lg"
          bg="gray.50"
          _placeholder={{ color: "gray.400" }}
        />
      </Field>
    </Fieldset.Content>

                {/* Успешное сообщение */}
                {okMessage && (
                    <Box mt={4} p={2} bg="green.100" borderRadius="md">
                        <Text color="green.700">{okMessage}</Text>
                    </Box>
                )}

                {/* Ошибки */}
                {errors.length > 0 &&
                    errors.map((err, idx) => (
                        <Text key={idx} color="red.500" fontSize="sm" mt={1}>
                            {err}
                        </Text>
                    ))}

                <Button
                    variant="solid"
                    onClick={handleSubmit}
                    isLoading={isLoading}
                    loadingText="Отправка..."
                    width="full"
                    mt={4}
                    px={6}
                    py={3}
                    rounded="lg"
                    bg="green"
                    color="white"
                >
                    Отправить отзыв
                </Button>
            </Fieldset.Root>
        </Box>
    );
}